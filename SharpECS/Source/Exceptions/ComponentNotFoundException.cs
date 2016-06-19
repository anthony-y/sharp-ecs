using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS.Exceptions
{
    class ComponentNotFoundException : Exception
    {
        public ComponentNotFoundException(Entity occuredIn)
            : base($"Component not found in Entity \"{occuredIn.Id}\".")
        {

        }
    }
}
