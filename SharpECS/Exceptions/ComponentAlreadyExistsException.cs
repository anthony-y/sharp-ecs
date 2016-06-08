using System;

namespace SharpECS.Exceptions
{
    class ComponentAlreadyExistsException : Exception
    {
        public ComponentAlreadyExistsException(Entity entity)
            : base($"Component already exists on entity \"{entity.Id}\".")
        {

        }
    }
}