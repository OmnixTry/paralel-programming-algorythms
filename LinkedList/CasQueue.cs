using System;
using System.Threading;

namespace LinkedList
{
	public class CasQueue<T>
	{
		public CasQueue(){
			var node = new QueueNode<T>();
			Start = node;
			End = node;
		}

		QueueNode<T> Start;

		QueueNode<T> End;

		public void Add(T data)
		{
			var newNode = new QueueNode<T>(data);

			while(true)
			{
				var end = End;
				var next = end.Next;

				if (end == End)
				{
					if (next == null)
					{
						if (CAS(ref end.Next, next, newNode))
						{
							break;
						}
					}
					else
					{
						CAS(ref End, end, next);
					}
				}
			}
		}

		public bool Remove(out T value)
		{
			QueueNode<T> start;

			while (true)
			{
				start = Start;
				QueueNode<T> next = start.Next;
				QueueNode<T> end = End;
				if (start == Start)
				{
					if (start == end)
					{
						if (next == null)
						{
							value = default(T);
							return false;
						}

						CAS(ref End, end, next);
					}
					else
					{
						value = next.Data;

						if (CAS(ref Start, start, next))
						{
							return true;
						}
					}
				}				
			}
		}

		private bool CAS(ref QueueNode<T> destination, QueueNode<T> compared, QueueNode<T> exchange)
		{
			return compared == Interlocked.CompareExchange(ref destination, exchange, compared);
		}
	}
}
