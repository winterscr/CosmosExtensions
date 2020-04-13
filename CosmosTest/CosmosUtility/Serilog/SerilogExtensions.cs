using System;
using System.Linq;
using Microsoft.Azure.Cosmos.Linq;
using Serilog;
using Serilog.Events;

namespace CosmosTest.CosmosUtility.Serilog
{
    public static class SerilogExtensions
    {
        public static void LogQuery<T>(this ILogger logger, IQueryable<T> query, LogEventLevel logLevel)
        {
            // Only log if needed to save building the query text
            if (logger.IsEnabled(logLevel))
            {
                logger.ForContext<LogQueryDetail>().Write(logLevel, "Cosmos query: {Query}", query.ToQueryDefinition().QueryText);
            }
        }

        public static IQueryable<T> Log<T>(this IQueryable<T> query, ILogger logger, LogEventLevel logLevel)
        {
            LogQuery(logger, query, logLevel);

            return query;
        }
    }
}