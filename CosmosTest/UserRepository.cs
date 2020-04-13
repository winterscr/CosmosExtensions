using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosTest.CommonExceptions;
using CosmosTest.CosmosUtility;
using CosmosTest.CosmosUtility.Serilog;
using CosmosTest.EntityModels;
using Microsoft.Azure.Cosmos;
using Serilog;
using Serilog.Events;
using User = CosmosTest.DomainModels.User;

namespace CosmosTest
{
    /// <summary>
    ///     A sample user repository.
    /// </summary>
    public class UserRepository : IUserRepository, IDisposable
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly CosmosClient _client;
        private readonly Container _container;

        private IQueryable<UserEntity> UserQuery => _container.GetItemLinqQueryable<UserEntity>();


        public UserRepository(IMapper mapper, ILogger logger)
        {
            _mapper = mapper;
            _logger = logger.ForContext<UserRepository>();
            
            // Normally pass the connection string in an IOptions or inject a named/keyed client but omitted for clarity
            _client = new CosmosClient(
                "AccountEndpoint=https://general-data.documents.azure.com:443/;AccountKey=yWGHqmL6AvcMqPbWFmmeIr2dvHQgdZxAbUamE05Yibkzjig2wDLlKwPU1jHZhRTMcZJv8wSxdyre6eGCxair2A==;");
            _container = _client.GetContainer("general", "users");
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        /// <summary>
        ///     Get all users from the repository.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<User> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            var query = Queries.QueryAll(UserQuery);


            // Can't use ProjectTo as we're using a custom value converter that can't be executing remotely
            // so using the Mapped variant of the helpers to perform mapping client side
            // We use WhereIsNotArchived here and not in the query as it would render the query untestable using unit tests.
            return query
                   //.ProjectTo<User>(_mapper.ConfigurationProvider)
                   .WhereIsNotArchived()
                   .Log(_logger, LogEventLevel.Verbose)
                   .ToMappedAsyncEnumerable(_mapper.Map<User>, cancellationToken);
        }

        /// <summary>
        ///     Get a specific user from the repository by their id. Returns null if not found.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The user or null if not found.</returns>
        public Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var query = Queries.QueryById(UserQuery, userId);

            // Can't use ProjectTo as we're using a custom value converter that can't be executing remotely
            // so using the Mapped variant of the helpers to perform mapping client side
            // We use WhereIsNotArchived here and not in the query as it would render the query untestable using unit tests.
            return query
                   //.ProjectTo<User>(_mapper.ConfigurationProvider)
                   .WhereIsNotArchived()
                   .Log(_logger, LogEventLevel.Verbose)
                   .MappedFirstOrDefaultAsync(_mapper.Map<User>, cancellationToken);
        }

        /// <summary>
        ///     Insert a new user.
        /// </summary>
        /// <param name="user">The user to insert.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ExistsException">Thrown if a conflicting record already exists.</exception>
        /// <exception cref="RepositoryException">Thrown if a general failure occurs talking to the repository.</exception>
        /// <exception cref="ValidationException">Thrown if the user object does not validate correctly.</exception>
        public async Task InsertAsync(User user, CancellationToken cancellationToken)
        {
            if (user.Id == null)
            {
                throw new ValidationException($"The property '{nameof(user.Id)}' cannot be null");
            }

            try
            {
                var userEntity = _mapper.Map<UserEntity>(user);

                await _container.CreateItemAsync(userEntity, cancellationToken: cancellationToken);
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.Conflict)
            {
                throw new ExistsException("A user with that id already exists", e);
            }
            catch (CosmosException e)
            {
                throw new RepositoryException("There was an error inserting the value into the repository", e);
            }
        }

        // Internal class to encapsulate the queries so they can be tested.
        internal static class Queries
        {
            public static IQueryable<UserEntity> QueryAll(IQueryable<UserEntity> source)
            {
                return source.WhereCommonAttributed().WhereInPartition(CosmosEntity.DefaultPartition);
            }

            public static IQueryable<UserEntity> QueryById(IQueryable<UserEntity> source, string userId)
            {
                return source
                       .WhereCommonAttributed()
                       .Where(i => i.Id == userId);
            }
        }
    }
}