using SkipList.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkipList.Impl
{
	public abstract class SafeStructBase<T> : ISafeStructType<T>
	{
		private long currentValue;

		protected SafeStructBase(T initialValue)
		{
			currentValue = ConvertTypeToLong(initialValue);
		}
		public T GetValue()
		{
			return ConvertLongToType(Interlocked.Read(ref currentValue));
		}

		public T SetValue(T newValue)
		{
			return ConvertLongToType(Interlocked.Exchange(ref currentValue, ConvertTypeToLong(newValue)));
		}

		protected abstract T ConvertLongToType(long value);
		protected abstract long ConvertTypeToLong(T value);

	}
}
