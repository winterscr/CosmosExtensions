using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Linq;

namespace CosmosTest.CosmosUtility
{
    /// <summary>
    ///     A collection of useful extensions for working with Cosmos queries.
    /// </summary>
    public static class CosmosExtensions
    {
        /// <summary>
        ///     Execute a cosmos query as an <see cref="IAsyncEnumerable{T}" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(
            this IQueryable<T> source,
            CancellationToken cancellationToken)
            where T : notnull
        {
            return ToMappedAsyncEnumerable(source, i => i, cancellationToken);
        }

        /// <summary>
        ///     Execute a cosmos query as an <see cref="IAsyncEnumerable{T}" /> and map the resulting objects.
        ///     If using AutoMapper then consider using ProjectTo() for simple projection instead, unless performing value conversion
        ///     or other complex maps.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="mapFunc"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<TResult> ToMappedAsyncEnumerable<TEntity, TResult>(
            this IQueryable<TEntity> source,
            Func<TEntity, TResult> mapFunc,
            [EnumeratorCancellation] CancellationToken cancellationToken)
            where TResult : notnull
        {
            var iterator = source.ToFeedIterator();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                foreach (TEntity item in response)
                {
                    yield return mapFunc(item);
                }
            }
        }

        /// <summary>
        ///     Execute a cosmos query asynchronously only retrieving a single value or null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<T?> FirstOrDefaultAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken)
            where T : class
        {
            return MappedFirstOrDefaultAsync(source, i => i, cancellationToken);
        }

        /// <summary>
        ///     Execute a cosmos query asynchronously only retrieving the first value or null.
        ///     The result is mapped if not null.
        ///     If using AutoMapper then consider using ProjectTo() for simple projection instead, unless performing value conversion
        ///     or other complex maps.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="mapFunc"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<TResult?> MappedFirstOrDefaultAsync<TEntity, TResult>(this IQueryable<TEntity> source,
                                                                                       Func<TEntity, TResult> mapFunc,
                                                                                       CancellationToken cancellationToken)
            where TResult : class
        {
            var feedIterator = source.Take(1).ToFeedIterator();

            if (feedIterator.HasMoreResults)
            {
                TEntity result = (await feedIterator.ReadNextAsync(cancellationToken)).FirstOrDefault();

                if (result != null)
                {
                    return mapFunc(result);
                }
            }

            return default;
        }

       /// <summary>
        ///     Include common predicates where the type of the entity matches and the version is at most as specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// \
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <param name="versionAtMost"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereCommon<T>(this IQueryable<T> source, string type, int versionAtMost)
            where T : CosmosEntity
        {
            return source.Where(i => i.Type == type && i.Version <= versionAtMost);
        }

        /// <summary>
        ///     Include common predicates to the query to filter on the type of the entity and max version of the entity version.
        ///     The entity type must be attributed with <see cref="CosmosEntityInfoAttribute" />.
        ///     This overload will return all entries that match, ignoring the IsArchived flag if present on the entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereCommonAttributed<T>(this IQueryable<T> source)
            where T : CosmosEntity
        {
            CosmosEntityInfoAttribute? attribute = typeof(T).GetCustomAttribute<CosmosEntityInfoAttribute>()
                                                   ?? throw new InvalidOperationException(
                                                       $"The entity type {typeof(T)} must have a {nameof(CosmosEntityInfoAttribute)} attribute");

            int version = attribute.Version;
            string type = attribute.Type;

            return source.Where(i => i.Type == type && i.Version <= version);
        }

        /// <summary>
        ///     Include only entities that are not archived. See remarks for information on unit testing.
        /// </summary>
        /// <remarks>
        ///     If you want to unit test queries, then use <see cref="WhereIsNotArchived{T}"/> or <see cref="WhereIsArchived{T}"/> in the repository
        ///     prior to execution only and not in the testable query.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIsNotArchived<T>(this IQueryable<T> source)
            where T : IArchivedEntity
        {
            return source.Where(i => !i.IsArchived.IsDefined() || !i.IsArchived);
        }

        /// <summary>
        ///     Include only entities that are archived. See remarks for information on unit testing.
        /// </summary>
        /// <remarks>
        ///     If you want to unit test queries, then use <see cref="WhereIsNotArchived{T}"/> or <see cref="WhereIsArchived{T}"/> in the repository
        ///     prior to execution only and not in the testable query.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIsArchived<T>(this IQueryable<T> source)
            where T : IArchivedEntity
        {
            return source.Where(i => i.IsArchived.IsDefined() && i.IsArchived);
        }

        /// <summary>
        ///     Include only entities with the specific type and version.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereTypeAndVersion<T>(this IQueryable<T> source, string type, int version)
            where T : CosmosEntity
        {
            return source.Where(i => i.Type == type && i.Version == version);
        }

        /// <summary>
        ///     Include only entities of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereType<T>(this IQueryable<T> source, string type)
            where T : CosmosEntity
        {
            return source.Where(i => i.Type == type);
        }

        /// <summary>
        ///     Include only entities at the specified version.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereVersion<T>(this IQueryable<T> source, int version)
            where T : CosmosEntity
        {
            return source.Where(i => i.Version == version);
        }

        /// <summary>
        ///     Include only entities where the version is higher or equal to the specified version.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereVersionAtLeast<T>(this IQueryable<T> source, int version)
            where T : CosmosEntity
        {
            return source.Where(i => i.Version >= version);
        }

        /// <summary>
        ///     Include only entities where the version is lower or equal to the specified version.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereVersionAtMost<T>(this IQueryable<T> source, int version)
            where T : CosmosEntity
        {
            return source.Where(i => i.Version <= version);
        }

        /// <summary>
        ///     Limits the query to the specified partition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="partition"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereInPartition<T>(this IQueryable<T> source, string partition)
            where T : CosmosEntity
        {
            return source.Where(i => i.Partition == partition);
        }
    }
}