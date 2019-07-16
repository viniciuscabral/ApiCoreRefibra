using System;


namespace WsProject.Model
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() : base() { }
        public UnauthorizedException(string message) : base(message) { }
    }
}
