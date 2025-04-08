using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public class DbContextFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public DbContextFactory(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public ApplicationDbContext CreateDbContext()
        {
            var provider = _httpContextAccessor.HttpContext?.Items["DbProvider"]?.ToString()?.ToLower() ?? "sqlserver";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            if (provider == "postgresql")
            {
                optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSQL"));
            }
            else if (provider == "mysql")
            {
                optionsBuilder.UseMySql(
                _configuration.GetConnectionString("MySQL"),
                new MySqlServerVersion(new Version(10, 4, 32)) 
);
            }
            else
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("SQLServer"));
            }

            return new ApplicationDbContext(optionsBuilder.Options, _configuration);
        }
    }
}
