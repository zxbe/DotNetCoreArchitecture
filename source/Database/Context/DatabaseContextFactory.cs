using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace DotNetCoreArchitecture.Database.Context
{
    public sealed class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            const string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Database;Integrated Security=true;Connection Timeout=10;";

            var builder = new DbContextOptionsBuilder<DatabaseContext>();

            builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));

            return new DatabaseContext(builder.Options);
        }
    }
}
