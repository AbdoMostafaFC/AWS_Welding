using MongoDB.Driver;
using WebApplication5.Controllers;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.DTO
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMongoCollection<User> _users;

        public BasicAuthMiddleware(RequestDelegate next, IMongoClient client)
        {
            _next = next;
            var database = client.GetDatabase("awsweld");
            _users = database.GetCollection<User>("User");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            
            if (path == "/api/Account/Login" || path == "/api/Account/register")
            {
                await _next(context);  // Skip authentication and continue with the request
                return;
            }

            // Check if the 'username' and 'password' headers are present
            if (!context.Request.Headers.TryGetValue("username", out var username) ||
                !context.Request.Headers.TryGetValue("password", out var password))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Username or password missing.");
                return;
            }

            // Validate username and password against the database
            var userExists = _users.Find(u => u.UserName == username && u.Passsword == password).Any();
            if (!userExists)
            {
                context.Response.StatusCode = 401; 
                await context.Response.WriteAsync("Invalid username or password.");
                return;
            }

            // If authentication is successful, call the next middleware in the pipeline
            await _next(context);
        }


    }
}
