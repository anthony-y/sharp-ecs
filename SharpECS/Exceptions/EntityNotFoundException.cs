using System;

namespace SharpECS.Exceptions
{
    class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(EntityPool pool)
            : base($"Entity not found in pool \"{pool.Name}\".")
        {
            
        }
    }
}