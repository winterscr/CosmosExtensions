using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using CosmosTest.CosmosUtility.Serilog;
using CosmosTest.DomainModels;
using Serilog;
using Serilog.Core;
using Serilog.Filters;
using Serilog.Sinks.SystemConsole.Themes;

[assembly: InternalsVisibleTo("CosmosTest.UnitTests")]

namespace CosmosTest
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // NOTE: This example uses autofac, serilog and automapper. These are optional.
            // Entity objects are also inheriting from CosmosEntity, this is also optional.

            // Set up the logger
            // Change this value to log queries out
            var logQueries = true;

            Logger logger = new LoggerConfiguration()
                            .MinimumLevel.Verbose()
                            .Filter.ByExcluding(i => !logQueries && Matching.FromSource<LogQueryDetail>()(i))
                            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                            .CreateLogger();

            Log.Logger = logger;

            // Set up the container
            var builder = new ContainerBuilder();
            builder.AddAutoMapper(config => config.AddMaps(Assembly.GetExecutingAssembly()));
            builder.RegisterInstance(Log.Logger).As<ILogger>();
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            
            await using IContainer container = builder.Build();

            try
            {
                // Get a repo and query it for users
                var userRepository = container.Resolve<IUserRepository>();

                // Uncomment to insert a test row
                //await userRepository.InsertAsync(new User {Id = "1234", Username = "Mr Flibble", Age = 20});

                await foreach (User user in userRepository.GetAllUsersAsync())
                {
                    logger.Information("User: {@User}", user);
                }

                User? userResult = await userRepository.GetUserByIdAsync("1234");
                logger.Information("Got user: {@User}", userResult);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error executing");
            }

            Console.WriteLine("Press return to exit");
            Console.ReadLine();
        }
    }
}