using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1
{
	class ListNode<DataType>
	{
		public DataType Data { get; set; }

		public ListNode<DataType> Next;

		public ListNode(DataType data)
		{
			Data = data;
		}
	}
}
