using AjeeviIoT.EntitiesDTO;
using AjeeviIoT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AjeeviIoT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ferrodbContext _ferrodbContext;

        public AuthController(IConfiguration configuration, ferrodbContext ferrodbContext)
        {
           // _userManager = userManager;
            _configuration = configuration;
            _ferrodbContext = ferrodbContext;
        }

        [HttpPost("login")]
        public IActionResult LoginData([FromBody] Login user)
        {
            if (user is null)
            {
                return BadRequest("Invalid user request!!!");
            }

            var rData = _ferrodbContext.UserLogins.Where(w => w.Email == user.UserName && w.Password == user.Password)?.FirstOrDefault();

            if (rData == null)
            {
                return Unauthorized();
            }


            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(issuer: _configuration["JwtSettings:Issuer"], audience: _configuration["JwtSettings:Audience"],
            claims: new List<Claim>(), expires: DateTime.Now.AddDays(30), signingCredentials: signinCredentials);
            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            //var ccData = _ferrodbContext.Deviceinstallations.Where(d => d.EntityId == rData.EntityId).Select(d => d.DeviceId).ToArray();

            //List<int> deviceTypeIds = new List<int>();
            //foreach (var c in ccData)
            //{
            //    int dtId = (int)_ferrodbContext.Devices.Where(w => w.Id == c).FirstOrDefault().Devicetypeid;
            //    if (dtId != null && dtId > 0)
            //    {
            //        deviceTypeIds.Add(dtId);
            //    }
            //}


            return Ok(new
            {
                access = tokenString,
                msg = "Login Successfully...",
                role = _ferrodbContext.Userroles.Where(w => w.Id == rData.UserroleId)?.FirstOrDefault()?.RoleType,
                entity_id = new
                {
                    id = rData.UserId,
                    user_name = rData.UserFname,
                    entity_id = rData.EntityId,
                    images = _ferrodbContext.Imagedetails.Where(i => i.ImageTypeId == rData.EntityId)?.FirstOrDefault()?.Imageurl,
                   // deviceTypeId = deviceTypeIds.Distinct()

                }

            });

            //return Ok(new LoginModel
            //{
            //    //Token = tokenString,
            //    access = tokenString,
            //    msg = "Login Successfully...",

            //    Id = rData.UserId,
            //    Name = rData.UserFname,
            //    Email = rData.Email,
            //    Mobile = rData.MobileNo,
            //    //role = rData.Role.RoleType,
            //    //UserName = rData.Email,
            //    Role = rData.RoleId?.ToString(),
            //    EntityId= rData.EntityId
            //});
        }

        public class LoginModel
        {

            public long Id { get; set; }

            public string Name { get; set; } = "";

            public string Email { get; set; } = "";

            public string UserName { get; set; } = "";

            public string Password { get; set; } = "";

            public string Mobile { get; set; } = "";

            public string UserLevel { get; set; } = "";
            //public string Token { get; set; } = "";
            public string access { get; set; } = "";
            public string msg { get; set; } = "";
            public string Role { get; set; } = "";
            //public string role { get; set; } = "";
            public int? EntityId { get; set; }
            public Entity Entity { get; set; }
        }
        public class Login
        {
            public string? UserName
            {
                get;
                set;
            }
            public string? Password
            {
                get;
                set;
            }
        }
        public class JWTTokenResponse
        {
            public string? Token
            {
                get;
                set;
            }
        }
    }
}
