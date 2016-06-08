using System;

namespace SharpECS.Exceptions
{
    class DuplicateEntityException : Exception
    {
        public DuplicateEntityException(EntityPool pool)
            : base($"Two entities in pool {pool.Id} shared the same tag.")
        {

        }
    }
}