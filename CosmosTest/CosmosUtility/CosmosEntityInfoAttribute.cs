using System;

namespace CosmosTest.CosmosUtility
{
    /// <summary>
    ///     Can be used to optionally mark <see cref="CosmosEntity"/> derived classes with metadata.
    ///     This metadata can be automatically added to queries for filtering using
    ///     <see cref="CosmosExtensions.WhereCommonAttributed{T}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CosmosEntityInfoAttribute : Attribute
    {
        public string Type { get; }
        public int Version { get; }

        public CosmosEntityInfoAttribute(string type, int version)
        {
            Type = type;
            Version = version;
        }
    }
}