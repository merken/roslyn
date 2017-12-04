using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moksy.core
{
    public static class MokParams
    {
        public static T Any<T>()
        {
            return default(T);
        }
    }
}
