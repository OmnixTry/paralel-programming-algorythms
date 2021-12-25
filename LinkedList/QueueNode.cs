using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedList
{
	public class QueueNode<T>
	{
		public T Data { get; set; }

		public QueueNode<T> Next;

		public QueueNode() 
		{
			Next = null;
			Data = default(T);
		}

		public QueueNode(T data, QueueNode<T> next = null)
		{
			Data = data;
			Next = next;
		}
	}
}
