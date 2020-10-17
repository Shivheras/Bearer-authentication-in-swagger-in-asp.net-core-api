using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using apis.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;

namespace apis.Controllers
{
    public class AuthenticateController : BaseController
    {
        private IConfiguration _config;

        public AuthenticateController(IConfiguration config)
        {
            _config = config;
        }
        
        private string GenerateJSONWebToken(LoginModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class Salt
        {
            public static string Create()
            {
                byte[] randomBytes = new byte[128 / 8];
                using (var generator = RandomNumberGenerator.Create())
                {
                    generator.GetBytes(randomBytes);
                    return Convert.ToBase64String(randomBytes);
                }
            }
        }
        // GET: Password
        public class Hash
        {
            public static string Create(string value, string salt)
            {
                var valueBytes = KeyDerivation.Pbkdf2(
                                    password: value,
                                    salt: Encoding.UTF8.GetBytes(salt),
                                    prf: KeyDerivationPrf.HMACSHA512,
                                    iterationCount: 10000,
                                    numBytesRequested: 256 / 8);

                return Convert.ToBase64String(valueBytes);
            }

            public static bool Validate(string value, string salt, string hash)
                => Create(value, salt) == hash;
        }
        private async Task<LoginModel> AuthenticateUser(LoginModel login)
        {
            LoginModel user = new LoginModel();
            SqlConnection constr = new SqlConnection(@"Data Source=PC0443\MSSQL2017;Initial Catalog=FileUpload;Integrated Security=True;MultipleActiveResultSets=True");
            constr.Open();

            string CommandText = "SELECT Username, Password,Salt FROM Users Where Username='"+login.UserName+"'";
            SqlCommand SqlCommand = new SqlCommand(CommandText, constr);
             var reader=  SqlCommand.ExecuteReader();
              while(reader.Read())
                {
                user.UserName = reader["Username"].ToString();
                user.Password = reader["Password"].ToString();
                user.Salt = reader["Salt"].ToString();
                
            }
            if (login.UserName == user.UserName &&  Hash.Validate(login.Password, user.Salt, user.Password)==true)
            {

                user = new LoginModel { UserName = user.UserName, Password = user.Password };


            }

            else
            {
                user = null;
            }

            constr.Close();
              
            return user;
        }
        [HttpPost(nameof(Register))]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] LoginModel data)
        {
            SqlConnection constr = new SqlConnection(@"Data Source=PC0443\MSSQL2017;Initial Catalog=FileUpload;Integrated Security=True;MultipleActiveResultSets=True");
            constr.Open();
            var salt = Salt.Create();
            var hash = Hash.Create(data.Password, salt);
            string CommandText = "INSERT INTO Users " +
                          "(Username,Password,Salt) " +
                          "VALUES ('" + data.UserName.ToString() + "','" + hash.ToString() + "','" + salt.ToString() + "')";

            SqlCommand SqlCommand = new SqlCommand(CommandText, constr);
            var reader = SqlCommand.ExecuteNonQuery();
            constr.Close();
            //response = Ok();
            return Ok(reader);
        }

        [AllowAnonymous]
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login([FromBody] LoginModel data)
        {
            IActionResult response = Unauthorized();
            var user = await AuthenticateUser(data);
            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(tokenString);
            }
            return response;
        }
        
    }
}