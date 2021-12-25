using System;

namespace SkipList
{
	public class SkipList<T>
	{
		#region FieldsAndproperties
		public const int MinLevel = 0;
        public const int MaxLevel = 12;

        public Node<T> Head { get; private set; } = new Node<T>(int.MinValue);
        public Node<T> Tail { get; private set; } = new Node<T>(int.MaxValue);

		#endregion
		public SkipList()
        {
            for (var i = 0; i < Head.Next.Length; ++i)
            {
                Head.Next[i] = new MarkedReference<Node<T>>(Tail, false);
            }
        }

        public void PrintListState()
		{
            var current = Head.Next[MinLevel].Value;
            while(current != Tail)
			{
                Console.Write(current.NodeKey);
                Console.Write(' ');
                current = current.Next[MinLevel].Value;
            }
		}

		#region SkipListMethods
		/*
            Якщо знайдено вузол із таким самим значенням, що й значення, яке потрібно вставити, тоді
             нічого не слід робити (математичний набір не може містити дублікатів).
             В іншому випадку ми повинні створити новий вузол і вставити його в список.
        */
		public bool Insert(Node<T> node)
        {
            var (succs, preds) = CreateNeighbourArrays();

            while (true)
            {
                if (Find(node, ref preds, ref succs))
                    return false; // can't insert when node is already there

                ConnectToSuccessors(node, succs); // link all the Nexts of the current node

                var pred = preds[MinLevel];
                var succ = succs[MinLevel];
                node.Next[MinLevel]  // at the botom level it's just a regular linked list
                    = new MarkedReference<Node<T>>(succ, false); // connect to the direct successor

                // in case if predecessor has changed by other threads while we preformed other operations
                // we need to repeat the process
                if (!IsCompareExchangedSuccessful(pred, MinLevel, node, succ))
                    continue;

                // connect current node to all the predecessors
                IterateOverAllLevelsUp(node, preds, succs);
                return true;
            }
        }        

        public bool Delete(Node<T> node)
        {
            var (succs, preds) = CreateNeighbourArrays();

            while (true)
            {
                if (!Find(node, ref preds, ref succs))
                    return false; // check nodes presence, can't delete nonexisting node

                // mark all the references of the node
                // this makes it logically deleted
                IterateOverAllLevelDown(node);
                var marked = false;
                var succ = node.Next[MinLevel].Get(ref marked);

                // deletes from the list
                return Remove(succ, node, succs, preds, ref marked);
            }
        }

        public bool Find(Node<T> node, ref Node<T>[] preds, ref Node<T>[] succs)
        {
            var isMarked = false;
            var isRetryNeeded = false;
            Node<T> current = null;

            while (true)
            {
                var pred = Head;
				bool isLookSuccessful = true;
                // finds the proper element on every level
                // if necessary deletes marked refernces
                // returns value after it reaches the bottom level
                for (var level = MaxLevel; level >= MinLevel; level--)
                {
                    current = pred.Next[level].Value;

                    isLookSuccessful = LookThroughMarked(level, ref isMarked, ref current, ref pred, node, isRetryNeeded);
                    if (!isLookSuccessful)
                        break;
                    
                    preds[level] = pred;
                    succs[level] = current;
                }

                if (!isLookSuccessful)
                    continue;

                return current != null 
                    && (current.NodeKey == node.NodeKey);
            }
        }
		#endregion

		#region PrivateHelperMethods
		private bool LookThroughMarked(int level, ref bool marked, ref Node<T> curr, ref Node<T> pred, Node<T> node,
            bool isRetryNeeded)
        {
            // loop goes through all the nexts on a current lefel
            // and as long as the nexts are marked
            // he skips them and connects predecessor to the marked reference successor
            while (true)
            {
                var succ = curr.Next[level].Get(ref marked);
                while (marked)
                {
                    // if something has been changed by other threads
                    // need to return and rerty 
                    if (!IsCompareExchangedSuccessful(pred, level, succ, curr))
                    {
                        return false;
                    }

                    curr = pred.Next[level].Value;
                    succ = curr.Next[level].Get(ref marked);
                }

                if (curr.NodeKey < node.NodeKey)
                {
                    pred = curr;
                    curr = succ;
                }
                else
                {
                    break;
                }
            }
            return true;
        }        

        private (Node<T>[], Node<T>[]) CreateNeighbourArrays()
        {
            var successors = new Node<T>[MaxLevel + 1];
            var predescessrs = new Node<T>[MaxLevel + 1];
            return (successors, predescessrs);
        }

        private void ConnectToSuccessors(Node<T> node, Node<T>[] succs)
        {
            for (var level = MinLevel; level <= node.TopLevel; level++)
            {
                node.Next[level]
                    = new MarkedReference<Node<T>>(succs[level], false);
            }
        }

        private bool IsCompareExchangedSuccessful(Node<T> pred, int level, Node<T> node, Node<T> succ)
        {
            return pred
                .Next[level]
                .CompareAndExchange(node, false, succ, false);
        }

        /// <summary>
        /// Connects the node to predecessors on all the upper levels 
        /// </summary>
        private void IterateOverAllLevelsUp(Node<T> node, Node<T>[] preds, Node<T>[] succs)
        {
            for (var level = MinLevel + 1; level <= node.TopLevel; level++)
            {
                while (true)
                {
                    var pred = preds[level];
                    var succ = succs[level];

                    if (IsCompareExchangedSuccessful(pred, level, node, succ))
                    {
                        break;
                    }
                    // if some other thread changed the predecessors or successors
                    // we need to find new ones and repeat the linking process
                    Find(node, ref preds, ref succs);
                }
            }
        }

        /// <summary>
        /// Marks all the references of the node except bottom level
        /// </summary>
        private Node<T> IterateOverAllLevelDown(Node<T> node)
        {
            Node<T> succ = null;
            // go through all the levels top to bottom (not including bottom)
            // and mark all the nextReferences of it
            for (var level = node.TopLevel; level > MinLevel; level--)
            {
                var isMarked = false;
                succ = node.Next[level].Get(ref isMarked);

                // if already marked just continue
                // if changed by other process, read again and retry
                while (!isMarked)
                {
                    node.Next[level]
                        .CompareAndExchange(succ, true, succ, false);
                    succ = node.Next[level].Get(ref isMarked);
                }
            }

            return succ;
        }

        private bool Remove(Node<T> succ, Node<T> node, Node<T>[] succs, Node<T>[] preds, ref bool marked)
        {
            while (true)
            {
                // marks bottom reference
                var iMarkedIt = node.Next[MinLevel].CompareAndExchange(succ, true, succ, false);
                succ = succs[MinLevel].Next[MinLevel].Get(ref marked);

                // if mark ing was successfull calling find
                // find physically removes all the references to the node it searches
                // if they are marked
                if (iMarkedIt)
                {
                    Find(node, ref preds, ref succs);
                    return true;
                }

                if (marked)
                {
                    return false;
                }
            }
        }
		#endregion

		//static readonly int MAX_LEVEL = 4;
		//Node<T> head = new Node<T>(int.MinValue, MAX_LEVEL);
		//Node<T> tail = new Node<T>(int.MaxValue, MAX_LEVEL);
		//public SkipList() {
		//	for (int i = 0; i < head.next.Length; i++) {
		//		head.next[i]
		//		= new Reference<Node<T>>(tail, false);
		//	}
		//}

		//bool Add(T x)
		//{
		//	Random random = new Random();
		//	int topLevel = random.Next(0, MAX_LEVEL);
		//	int bottomLevel = 0;
		//	Node<T>[] preds = new Node<T>[MAX_LEVEL + 1];
		//	Node<T>[] succs = new Node<T>[MAX_LEVEL + 1];
		//	while (true)
		//	{
		//		bool found = find(x, preds, succs);
		//		if (found)
		//		{
		//			return false;
		//		}
		//		else
		//		{
		//			Node<T> newNode = new Node<T>(x, topLevel);
		//			for (int level = bottomLevel; level <= topLevel; level++)
		//			{
		//				Node<T> succ = succs[level];
		//				//newNode.next[level].set(succ, false);
		//				newNode.next[level] = Reference<Node<T>>.GetUnmarkedReference(succ);
		//			}
		//			Node<T> pred = preds[bottomLevel];
		//			Node<T> succElse = succs[bottomLevel];
		//			newNode.next[bottomLevel] = Reference<Node<T>>.GetUnmarkedReference(succElse);//.set(succ, false);
		//			if (!pred.next[bottomLevel].compareAndSet(succElse, newNode,
		//				false, false))
		//			{
		//				continue;
		//			}
		//			for (int level = bottomLevel + 1; level <= topLevel; level++)
		//			{
		//				while (true)
		//				{
		//					pred = preds[level];
		//					succElse = succs[level];
		//					if (pred.next[level].compareAndSet(succ, newNode, false, false))
		//						break;
		//					find(x, preds, succs);
		//				}
		//			}
		//				return true;
		//		}
		//	}
		//}
	}
}
