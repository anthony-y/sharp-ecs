using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSIsBetter.Exceptions
{
    class DuplicateEntityException : Exception
    {
        public DuplicateEntityException(EntityPool pool)
            : base($"Two entities in pool {pool.Name} shared the same tag.")
        {

        }
    }
}
