

namespace ApiRefibra.Exceptions
{
    public class DuplicateUserException : System.Exception
    {
        public DuplicateUserException() : base() { }
        public DuplicateUserException(string message) : base(message) { }
    }
}
