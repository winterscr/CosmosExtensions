using System;
using CosmosTest.CosmosUtility;
using Newtonsoft.Json;

namespace CosmosTest.EntityModels
{
    [CosmosEntityInfo(DataType, CurrentVersion)]
    public class UserEntity : CosmosEntity, IArchivedEntity
    {
        public const string DataType = "User";
        public const int CurrentVersion = 1;

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("username")]
        public string? Username { get; set; }

        [JsonProperty("age")]
        public int? Age { get; set; }

        // JSON property name will come from the interface
        public bool IsArchived { get; set; }

        public UserEntity() : base(CurrentVersion, DataType)
        {
        }
    }
}