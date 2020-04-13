using System;
using Newtonsoft.Json;

namespace CosmosTest.CosmosUtility
{
    /// <summary>
    ///     Provides an optional base class for Cosmos entities. Supports versioning and heterogeneous containers
    ///     along with some of the standard Cosmos fields.
    ///
    ///     Can be used with <see cref="CosmosExtensions.WhereType{T}"/>, <see cref="CosmosExtensions.WhereVersion{T}"/>
    ///     and similar methods.
    /// </summary>
    public abstract class CosmosEntity
    {
        /// <summary>
        ///     The default partition to use if one is not specified
        /// </summary>
        public const string DefaultPartition = "__undefined__";

        [JsonProperty("__version")]
        public int Version { get; }

        [JsonProperty("__type")]
        public string Type { get; }

        [JsonProperty("__partition")]
        public string Partition { get; set; }

        [JsonProperty("_ts")]
        public int? Timestamp { get; set; }

        [JsonProperty("_etag")]
        public string? ETag { get; set; }

        protected CosmosEntity(int version, string type)
            : this(version, type, DefaultPartition)
        {
        }

        protected CosmosEntity(int version, string type, string partition)
        {
            Version = version;
            Type = type;
            Partition = partition;
        }
    }
}