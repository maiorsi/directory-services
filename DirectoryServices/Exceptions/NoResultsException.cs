// <copyright file="NoResultsException.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System;

namespace DirectoryServices.Exceptions
{
    public class NoResultsException : Exception
    {
        public NoResultsException(string message) : base(message) { }
    }
}