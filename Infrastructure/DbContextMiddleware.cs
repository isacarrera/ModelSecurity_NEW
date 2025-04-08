using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Infrastructure
{
    public class DbContextMiddleware
    {
        private readonly RequestDelegate _next;

        public DbContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var dbProvider = context.Request.Headers["X-DB-Provider"].ToString().ToLower();

            if (string.IsNullOrEmpty(dbProvider))
                dbProvider = "sqlserver"; // default

            context.Items["DbProvider"] = dbProvider;

            await _next(context);
        }
    }
}
