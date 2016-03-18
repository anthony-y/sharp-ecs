using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSIsBetter.Exceptions
{
    class ComponentAlreadyExistsException : Exception
    {
        public ComponentAlreadyExistsException(Entity entity)
            : base("Component already exists on entity \"" + entity.Tag + "\".")
        {

        }
    }
}
