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

				// if nothing was changed by other threads
				if (end == End)
				{
					// if nothing was added after the end
					// attach new element to the queue
					if (next == null)
					{
						if (CAS(ref end.Next, next, newNode))
						{
							break;
						}
					}
					// if something was added to the end,
					// move it forward
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
				// if nothing is changed by others
				if (start == Start)
				{
					if (start == end)
					{
						// if no elements are present in the queue
						// return default value 
						// removing unsuccessfull
						if (next == null)
						{
							value = default(T);
							return false;
						}

						// if fsomething is attached after end, move end ointer forward
						CAS(ref End, end, next);
					}
					// if queue has more than 1 element 
					// return it's value and try move the start pointer
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
