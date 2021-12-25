using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarrisList
{
	class ListNode<DataType>
	{
		public DataType Data { get; set; }

		public bool IsMarked 
		{ 
			get 
			{ 
				if(Next == null)
				{
					return false;
				}
				return Next.IsMarked; 
			} 
		}

		public Reference<DataType> Next;

		public ListNode(DataType data = default(DataType))
		{
			Data = data;
		}


	}
}
