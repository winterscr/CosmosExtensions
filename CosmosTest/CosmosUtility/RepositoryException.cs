using System;
using System.Runtime.Serialization;
using Microsoft.Azure.Cosmos;

namespace CosmosTest.CosmosUtility
{
    /// <summary>
    ///     A general repository exception that can be thrown by a repository to wrap <see cref="CosmosException"/> exceptions
    ///     from the Cosmos client.
    /// </summary>
    [Serializable]
    public class RepositoryException : Exception
    {
        public RepositoryException()
        {
        }

        protected RepositoryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public RepositoryException(string? message) : base(message)
        {
        }

        public RepositoryException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}