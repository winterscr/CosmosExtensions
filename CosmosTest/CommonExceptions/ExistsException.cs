using System;
using System.Runtime.Serialization;

namespace CosmosTest.CommonExceptions
{
    /// <summary>
    ///     Thrown when something exists that shouldn't.
    /// </summary>
    public class ExistsException : Exception
    {
        public string? ConflictingId { get; set; }
        public object? ConflictingItem { get; set; }

        public ExistsException()
        {
        }

        protected ExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ExistsException(string? message) : base(message)
        {
        }

        public ExistsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        public ExistsException(string conflictingId, object conflictingItem)
        {
            ConflictingId = conflictingId;
            ConflictingItem = conflictingItem;
        }

        public ExistsException(string conflictingId, object conflictingItem, string? message) : base(message)
        {
            ConflictingId = conflictingId;
            ConflictingItem = conflictingItem;
        }

        public ExistsException(string conflictingId, object conflictingItem, string? message, Exception? innerException) : base(message,
            innerException)
        {
            ConflictingId = conflictingId;
            ConflictingItem = conflictingItem;
        }
    }
}