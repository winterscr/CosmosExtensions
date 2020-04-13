using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CosmosTest.CommonExceptions;
using CosmosTest.CosmosUtility;
using CosmosTest.DomainModels;

namespace CosmosTest
{
    public interface IUserRepository
    {
        /// <summary>
        ///     Get all users from the repository.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        IAsyncEnumerable<User> GetAllUsersAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        ///     Get a specific user from the repository by their id. Returns null if not found.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The user or null if not found.</returns>
        Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        ///     Insert a new user.
        /// </summary>
        /// <param name="user">The user to insert.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ExistsException">Thrown if a conflicting record already exists.</exception>
        /// <exception cref="RepositoryException">Thrown if a general failure occurs talking to the repository.</exception>
        /// <exception cref="ValidationException">Thrown if the user object does not validate correctly.</exception>
        Task InsertAsync(User user, CancellationToken cancellationToken = default);
    }
}