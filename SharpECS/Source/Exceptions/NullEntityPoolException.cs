using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS.Exceptions
{
    class NullEntityPoolException : Exception
    {
        public NullEntityPoolException(EntityPool entityPool)
            : base($"EntityPool {entityPool.Id} was null.")
        {

        }
    }
}
