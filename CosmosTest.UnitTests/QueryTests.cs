using System.Collections.Generic; 
using System.Linq;
using CosmosTest.CosmosUtility;
using CosmosTest.EntityModels;
using FluentAssertions;
using NUnit.Framework;

namespace CosmosTest.UnitTests
{
    [TestFixture]
    [Category("Unit")]
    public class QueryTests
    {
        [Test]
        public void GetAllUsers_HasUsers_ReturnsAllUsers()
        {
            var data = new List<UserEntity>
            {
                new UserEntity {Id = "anyid1", Age = 40, ETag = "AnyTag1", Partition = CosmosEntity.DefaultPartition, Username = "AnyUsername1" },
                new UserEntity {Id = "anyid2", Age = 20, ETag = "AnyTag2", Partition = CosmosEntity.DefaultPartition, Username = "AnyUsername2" }
            };
            var query = UserRepository.Queries.QueryAll(data.AsQueryable());
            var results = query.ToList();

            results.Should().BeEquivalentTo(data, "query should return all data");
        }

        [Test]
        public void GetAllUsers_HasNoUsers_ReturnsEmptyCollection()
        {
            var data = new List<UserEntity>();
            var query = UserRepository.Queries.QueryAll(data.AsQueryable());
            var results = query.ToList();

            results.Should().BeEquivalentTo(data, "query should return all data");
        }

        [Test]
        public void GetUserById_HasUser_ReturnsCorrectUser()
        {
            var data = new List<UserEntity>
            {
                new UserEntity {Id = "anyid1", Age = 40, ETag = "AnyTag1", Partition = CosmosEntity.DefaultPartition, Username = "AnyUsername1" },
                new UserEntity {Id = "anyid2", Age = 20, ETag = "AnyTag2", Partition = CosmosEntity.DefaultPartition, Username = "AnyUsername2" }
            };
            var query = UserRepository.Queries.QueryById(data.AsQueryable(), "anyid2");
            var results = query.ToList();

            results.Should().ContainSingle(i => i == data[1], "query should return correct user");
        }

        [Test]
        public void GetUserById_DoesNotHaveUser_ReturnsNull()
        {
            var data = new List<UserEntity>
            {
                new UserEntity {Id = "anyid1", Age = 40, ETag = "AnyTag1", Partition = CosmosEntity.DefaultPartition, Username = "AnyUsername1" },
                new UserEntity {Id = "anyid2", Age = 20, ETag = "AnyTag2", Partition = CosmosEntity.DefaultPartition, Username = "AnyUsername2" }
            };
            var query = UserRepository.Queries.QueryById(data.AsQueryable(), "missingid");
            var results = query.ToList();

            results.Should().BeEmpty("should have not have found any matches");
        }
    }
}