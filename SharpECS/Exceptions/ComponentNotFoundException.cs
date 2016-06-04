using System;

namespace SharpECS.Exceptions
{
    class ComponentNotFoundException : Exception
    {
        public ComponentNotFoundException(Entity occuredIn)
            : base($"Component not found in Entity \"{occuredIn.Tag}\".")
        {

        }
    }
}