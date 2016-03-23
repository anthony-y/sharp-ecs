using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS.Exceptions
{
    class ECSCacheException : Exception
    {
        public ECSCacheException()
            : base("A blank (cached) Entity was found in an EntityPool.")
        {

        }
    }
}
