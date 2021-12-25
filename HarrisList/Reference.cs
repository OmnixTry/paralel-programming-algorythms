using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarrisList
{
	class Reference<T>
	{
		public ListNode<T> Ref;

		public bool IsMarked { get; }

		public static Reference<T> GetMarkedReference(ListNode<T> reference)
		{
			return new Reference<T>(reference, true);
		}

		public static Reference<T> GetUnmarkedReference(ListNode<T> reference)
		{
			return new Reference<T>(reference, false);
		}

		public Reference(ListNode<T> reference, bool isMarked = false)
		{
			Ref = reference;
			IsMarked = isMarked;
		}
	}
}
