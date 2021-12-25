using SkipList.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkipList.Impl
{
    public class SafeBool: SafeStructBase<bool>
    {
        public SafeBool(bool initialValue): base(initialValue) {}
	
        protected override bool ConvertLongToType(long value)
		{
            return Convert.ToBoolean(value);
        }

		protected override long ConvertTypeToLong(bool value)
		{
            return Convert.ToInt32(value);
        }
	}

}
