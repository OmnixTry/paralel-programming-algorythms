using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkipList
{
    public class Reference<T> where T : class
    {
        private T _value;

        public Reference(T value)
        {
            _value = value;
        }

        public T Value
        {
            get
            {
                object obj = _value;
                return (T)Thread.VolatileRead(ref obj);
            }
        }

        public bool CompareAndExchange(T newValue, T oldValue)
        {
            return ReferenceEquals(Interlocked.CompareExchange(ref _value, newValue, oldValue), oldValue);
        }
    }
}
