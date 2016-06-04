using System;

namespace SharpECS.Exceptions
{
    class CacheException : Exception
    {
        public CacheException()
            : base("A blank (cached) Entity was found in an EntityPool.")
        {

        }
    }
}