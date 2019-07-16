using System;

namespace WsProject.Model
{
    public class DuplicateUserException : Exception
    {
        public DuplicateUserException() : base() { }
        public DuplicateUserException(string message) : base(message) { }
    }
}
