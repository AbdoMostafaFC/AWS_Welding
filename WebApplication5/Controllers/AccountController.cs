using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;
        public AccountController(IMongoClient client)
        {
            var database = client.GetDatabase("awsweld");
            _users = database.GetCollection<User>("User");
        }
        [HttpPost("register")]
        public IActionResult register(User user)
        {

           
            var existingUser = _users.Find(u => u.UserName == user.UserName).FirstOrDefault();
            if (existingUser != null)
            {
                return BadRequest(new { message = "Username already exists." });
            }

            _users.InsertOne(user);
            return Ok(user);


        }
        [HttpPost("Login")]
        public IActionResult LgoIn(User user)
        {
            
            var existingUser = _users.Find(u => u.UserName == user.UserName && u.Passsword == user.Passsword).FirstOrDefault();

            if (existingUser == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            
            return Ok(new { message = "Login successful." });


        }
    }



    public class User {
    
        public ObjectId Id { get; set; }
        public string UserName { get; set; }
        public string Passsword { get; set; }
    
    
    }
}
