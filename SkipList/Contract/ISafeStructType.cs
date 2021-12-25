using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkipList.Contract
{
	interface ISafeStructType<T>
	{
		T GetValue();
/*		T SetValue(T newValue);
*/	}
}
