using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS.Exceptions
{
    class IndependentEntityException : Exception
    {
        public IndependentEntityException(Entity entity)
            : base($"Entity \"{entity.Tag}\" does not belong to an EntityPool.")
        {

        }
    }
}
