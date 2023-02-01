// <copyright file="MultipleResultsException.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System;

namespace DirectoryServices.Exceptions
{
    public class MultipleResultsException : Exception
    {
        public MultipleResultsException(string message) : base(message) { }
    }
}