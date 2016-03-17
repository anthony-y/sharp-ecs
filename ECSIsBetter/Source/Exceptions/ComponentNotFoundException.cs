using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSIsBetter.Exceptions
{
    class ComponentNotFoundException : Exception
    {
        public ComponentNotFoundException(Entity occuredIn)
            : base($"Component not found in Entity \"{occuredIn.Tag}\".")
        {

        }
    }
}
