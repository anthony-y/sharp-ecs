using System;

namespace SharpECS.Exceptions
{
    class IndependentEntityException : Exception
    {
        public IndependentEntityException(Entity entity)
            : base($"Entity \"{entity.Id}\" does not belong to an EntityPool.")
        {

        }
    }
}