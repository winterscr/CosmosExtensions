using System;

namespace CosmosTest.DomainModels
{
    public class User
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public int? Age { get; set; }
        public DateTimeOffset? LastUpdated { get; set; }
        public string? ETag { get; set; }
    }
}