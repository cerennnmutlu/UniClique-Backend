using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace UniCliqueBackend.Persistence.Contexts
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var connectionString =
                Environment.GetEnvironmentVariable("ConnectionStrings__PostgreSql")
                ?? "Host=localhost;Port=5432;Database=uniclique_db;Username=uniclique_user;Password=uniclique_pass";

            optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("UniCliqueBackend.Persistence"));
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
