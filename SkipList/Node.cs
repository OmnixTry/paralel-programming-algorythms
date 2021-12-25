using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkipList
{
	public sealed class Node<T>
	{
        private static uint _randomSeed;
        public MarkedReference<T> NodeValue { get; }

        public int NodeKey { get; }

        public MarkedReference<Node<T>>[] Next { get; }

        public int TopLevel { get; }

        static Node()
        {
            _randomSeed = (uint)(DateTime.Now.Millisecond) | 0x0100;
        }

        public Node(int key)
        {
            NodeValue = new MarkedReference<T>(default(T), false);
            NodeKey = key;
            Next = new MarkedReference<Node<T>>[SkipList<T>.MaxLevel + 1];
            for (var i = 0; i < Next.Length; ++i)
            {
                Next[i] = new MarkedReference<Node<T>>(null, false);
            }

            TopLevel = SkipList<T>.MaxLevel;
        }

        public Node(T value, int key)
        {
            NodeValue = new MarkedReference<T>(value, false);
            NodeKey = key;
            var height = RandomLevel();
            Next = new MarkedReference<Node<T>>[height + 1];
            for (var i = 0; i < Next.Length; ++i)
            {
                Next[i] = new MarkedReference<Node<T>>(null, false);
            }

            TopLevel = height;
        }

        private static int RandomLevel()
        {
            Random rand = new Random((int)_randomSeed);
            return rand.Next(SkipList<T>.MaxLevel);
        }
        /*T value;
		readonly int key;
		readonly int MaxLevel;
		public Reference<Node<T>>[] next;
		private int topLevel;
		// constructor for sentinel nodes
		public Node(int key, int maxLevel)
		{
			value = null;
			MaxLevel = maxLevel;
			this.key = key;
			next = (Reference<Node<T>>[])new Reference<Node<T>>[MaxLevel + 1];
			for (int i = 0; i < next.Length; i++)
			{
				next[i] = new Reference<Node<T>>(null, false);
			}
			topLevel = maxLevel;
		}
		// constructor for ordinary nodes
		public Node(T x, int height)
		{
			value = x;
			key = x.GetHashCode();
			next = (Reference<Node<T>>[]) new Reference<Node<T>>[height + 1];
			for (int i = 0; i < next.Length; i++)
			{
				next[i] = new Reference<Node<T>>(null, false);
			}
			topLevel = height;
		}*/
    }
}

