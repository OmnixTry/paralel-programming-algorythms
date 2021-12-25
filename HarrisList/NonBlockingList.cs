using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HarrisList
{
	public class NonBlockingList<DataType> where DataType : IComparable
	{
		class SearchResult 
		{
			public ListNode<DataType> rightNode = null;
			public ListNode<DataType> leftNode = null;
			public Reference<DataType> leftNodeNext = null;
		}

		ListNode<DataType> Head { get; set; }
		ListNode<DataType> Tail { get; set; }

		public NonBlockingList()
		{
			Head = new ListNode<DataType>();
			Tail = new ListNode<DataType>();
			Head.Next = NewRef(Tail);
		}

		public bool Insert(DataType data)
		{
			ListNode<DataType> newNode = new ListNode<DataType>(data);
			SearchResult searchResult;
			
			while (true)
			{
				searchResult = Search(data); // TODO searchNode
				if (searchResult.rightNode != Tail && EqualityComparer<DataType>.Default.Equals(searchResult.rightNode.Data, data))
				{
					return false;
				}
				newNode.Next = NewRef(searchResult.rightNode);
				if(CAS(ref searchResult.leftNode.Next, searchResult.leftNodeNext, NewRef(newNode)))
				{
					return true;
				}
			}
		}

		public bool Delete(DataType data)
		{
			Reference<DataType> rightNodeNext = null;
			SearchResult searchResult;

			while (true)
			{
				searchResult = Search(data); //TODO Add search method
				if(searchResult.rightNode == Tail || !EqualityComparer<DataType>.Default.Equals(searchResult.rightNode.Data, data))
				{
					return false;
				}
				rightNodeNext = searchResult.rightNode.Next;
				if (!rightNodeNext.IsMarked)
				{
					if (CAS(ref searchResult.rightNode.Next, rightNodeNext, NewMarkedRef(rightNodeNext.Ref)))
					{
						break;
					}
				}
			}
			if (!CAS(ref searchResult.leftNode.Next, searchResult.leftNodeNext, rightNodeNext))
			{
				searchResult.rightNode = null; //TODO Search
			}
			return true;
		}

		public bool Find(DataType data)
		{
			SearchResult searchResult;

			searchResult = Search(data); //TODO search
			if(searchResult.rightNode == Tail || !EqualityComparer<DataType>.Default.Equals(searchResult.rightNode.Data, data))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public void PrintTraverse()
		{
			ListNode<DataType> node = Head;
			StringBuilder sb = new StringBuilder();
			while (node != null && node.Next.Ref != Tail)
			{
				node = node.Next.Ref;
				sb.Append(node.Data);
				sb.Append(' ');
			}
			Console.WriteLine(sb.ToString());
		}

		private SearchResult Search(DataType searchData)
		{
			var result = new SearchResult();

			while (true)
			{
				ListNode<DataType> node = Head;
				ListNode<DataType> nodeNext = Head.Next.Ref;

				do
				{
					if (!nodeNext.IsMarked)
					{
						result.leftNode = node;
						result.leftNodeNext = node.Next;
					}

					//node = Reference<DataType>.GetUnmarkedReference(nodeNext);
					node = nodeNext;
					if (node == Tail)
					{
						break;
					}
					nodeNext = node.Next.Ref;
				} while (node.Next.IsMarked || node.Data.CompareTo(searchData) == -1);
				result.rightNode = node;

				if(result.leftNodeNext != null && result.leftNodeNext.Ref == result.rightNode)
				{
					if(result.rightNode != Tail && result.rightNode.Next.IsMarked)
					{
						continue;
					}
					else
					{
						return result;
					}
				}

				if(result.leftNode != null && CAS(ref result.leftNode.Next, result.leftNodeNext, NewRef(result.rightNode)))
				{
					if (result.rightNode != Tail && result.rightNode.Next.IsMarked)
					{
						continue;
					}
				}
			}
		}

		private bool CAS(ref Reference<DataType> destination, Reference<DataType> compared, Reference<DataType> exchange)
		{
			return compared == Interlocked.CompareExchange(ref destination, exchange, compared);
		}

		private Reference<DataType> NewRef(ListNode<DataType> listNode)
		{
			return Reference<DataType>.GetUnmarkedReference(listNode);
		}

		private Reference<DataType> NewMarkedRef(ListNode<DataType> listNode)
		{
			return Reference<DataType>.GetMarkedReference(listNode);
		}
	}
}
