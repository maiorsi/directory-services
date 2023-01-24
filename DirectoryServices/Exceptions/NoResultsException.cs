using System;

namespace DirectoryServices.Exceptions
{
    public class NoResultsException : Exception
    {
        public NoResultsException(string message) : base(message) { }
    }
}