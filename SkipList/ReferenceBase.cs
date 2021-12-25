using SkipList.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkipList
{
    public class ReferenceBase<T>
    {
        public readonly T Value;
        public readonly SafeBool Marked;

        public ReferenceBase(T value, bool marked)
        {
            this.Value = value;
            this.Marked = new SafeBool(marked);
        }
    }
}
