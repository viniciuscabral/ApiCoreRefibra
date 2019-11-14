using System;

namespace ApiRefibra.Exceptions
{
    public class FusekiException : Exception
    {
        public FusekiException() : base() { }
        public FusekiException(string message) : base(message) { }
    }
}
