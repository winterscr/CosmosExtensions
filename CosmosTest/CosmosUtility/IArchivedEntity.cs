using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CosmosTest.CosmosUtility
{
    /// <summary>
    ///     Can be added to <see cref="CosmosEntity"/> derived classes that want to support
    ///     soft deletion in a consistent manner. Can be used with <see cref="CosmosExtensions.WhereIsArchived{T}"/>
    ///     and <see cref="CosmosExtensions.WhereIsNotArchived{T}"/>.
    /// </summary>
    public interface IArchivedEntity
    {
        [JsonProperty("__isArchived")]
        public bool IsArchived { get; set; }
    }
}