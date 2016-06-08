using System;

namespace SharpECS.Exceptions
{
    class NullEntityPoolException : Exception
    {
        public NullEntityPoolException(EntityPool entityPool)
            : base($"EntityPool {entityPool.Id} was null.")
        {

        }
    }
}