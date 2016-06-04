using System;

namespace SharpECS.Exceptions
{
    class NoCompatibleEntitiesException : Exception
    {
        public NoCompatibleEntitiesException()
            : base("No compatible entities found for system.")
        {

        }
    }
}