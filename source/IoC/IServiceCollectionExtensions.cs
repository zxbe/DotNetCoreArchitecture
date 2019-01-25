using DotNetCore.IoC;
using DotNetCoreArchitecture.Application;
using DotNetCoreArchitecture.Database;
using DotNetCoreArchitecture.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace DotNetCoreArchitecture.IoC
{
    public static class IServiceCollectionExtensions
    {
        public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCriptography(Guid.NewGuid().ToString());
            services.AddHash();
            services.AddJsonWebToken(Guid.NewGuid().ToString());
            services.AddLogger(configuration);

            services.AddClassesMatchingInterfacesFrom
            (
                Assembly.GetAssembly(typeof(IAuthenticationApplication)),
                Assembly.GetAssembly(typeof(IAuthenticationDomain)),
                Assembly.GetAssembly(typeof(IDatabaseUnitOfWork))
            );

            var connectionString = configuration.GetConnectionString(nameof(DatabaseContext));
            services.AddDbContextEnsureCreatedMigrate<DatabaseContext>(options => options.UseSqlServer(connectionString));
        }
    }
}
