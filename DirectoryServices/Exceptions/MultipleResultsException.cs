using System;

namespace DirectoryServices.Exceptions
{
    public class MultipleResultsException : Exception
    {
        public MultipleResultsException(string message) : base(message) { }
    }
}