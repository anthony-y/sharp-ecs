using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS.Exceptions
{
    class NoCompatibleEntitiesException : Exception
    {
        public NoCompatibleEntitiesException()
            : base("No compatible entities found for system.")
        {

        }
    }
}
