using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace testing.Controllers
{

    [ApiController]
    public class StudentController : ControllerBase
    {

        private readonly DataContext _applicationDbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        public StudentController(DataContext applicationDbContext, IHttpClientFactory httpClientFactory)
        {
            _applicationDbContext = applicationDbContext;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        [Route("api/app/facebook-login")]
        public async Task<IActionResult> FacebookLogin([FromBody] FacebookLoginModel model)
        {

            string graphApiEndpoint = "https://graph.facebook.com/v13.0/me";

            using (HttpClient httpClient = new HttpClient())
            {
                // Include the access token in the request headers
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {model.AuthToken}");

                // Make a request to the Facebook Graph API
                HttpResponseMessage response = await httpClient.GetAsync(graphApiEndpoint);

                // Validate the Facebook access token
                var facebookUser = await ValidateFacebookAccessToken(model.AuthToken);
                if (facebookUser == null)
                {
                    return BadRequest("Invalid Facebook access token");
                }

                // Check if the user already exists
                var existingUser = await _applicationDbContext.Students
                    .FirstOrDefaultAsync(x => x.Email == facebookUser.Email);

                if (existingUser != null)
                {
                   
                    var tokenString = GenerateToken(existingUser.Name, existingUser.Email, existingUser.Id);
                    return Ok(new { message = "User exists ", user = tokenString });
                }
                else
                {
                    // User doesn't exists create a new user
                    var newUser = new Student
                    {
                        Name = facebookUser.Name,
                        Email = facebookUser.Email,
                        Password = ""
                    };

                    var result = _applicationDbContext.Add(newUser);
                    await _applicationDbContext.SaveChangesAsync();


                    var tokenString = GenerateToken(newUser.Name, newUser.Email, newUser.Id);
                    return Ok(new { message = "Login successful", user = tokenString });
                }
            }
        }
        private string GenerateToken(string name, string email, int id)
        {
            // Define claims for the token
            var claims = new List<Claim>
        {
            new Claim("Name", name),
            new Claim("Email", email),
            new Claim("Id", id.ToString())
        };

            // Specify the key used to sign the token
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));

            // Create signing credentials using the key and the specified algorithm
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            // Set token options, including issuer, audience, claims, expiration, and signing credentials
            var tokenOptions = new JwtSecurityToken(
                issuer: "https://localhost:5001",
                audience: "https://localhost:5001",
                claims: claims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: signinCredentials
            );

            // Generate the JWT token
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return tokenString;
        }
        private async Task<FacebookUserModel> ValidateFacebookAccessToken(string authToken)
        {
            var userInformation = await GetFacebookUserFromGraphApi(authToken);
            if (userInformation != null)
            {
                // Map the received user information to your FacebookUserModel
                return new FacebookUserModel
                {
                    Id = userInformation["id"],
                    Name = userInformation["name"],
                    Email = userInformation["email"],
                    PhotoUrl = $"https://graph.facebook.com/{userInformation["id"]}/picture?type=normal",
                    FirstName = userInformation["first_name"],
                    LastName = userInformation["last_name"],
                    AuthToken = authToken
                };
            }

            return null;
        }

        private async Task<Dictionary<string, string>> GetFacebookUserFromGraphApi(string authToken)
        {
            try
            {
                // Configure HttpClient to make a request to the Facebook Graph API
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetStringAsync($"https://graph.facebook.com/v13.0/me?fields=id,name,email,first_name,last_name&access_token={authToken}");

                // Deserialize the JSON response to a Dictionary
                var userInformation = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

                return userInformation;
            }
            catch (Exception ex)
            {
                return  null;

            }
        }
        [HttpPost]
        [Route("api/app/google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginModel model)
        {
            var userInfo = await ValidateGoogleAccessToken(model.IdToken);
            if (userInfo != null)
            {
                // Check if the user already exists
                var existingUser = await _applicationDbContext.Students
                    .FirstOrDefaultAsync(x => x.Email == userInfo.Email);

                if (existingUser != null)
                {
                    var tokenString = GenerateToken(existingUser.Name, existingUser.Email, existingUser.Id);
                    return Ok(new { message = "User exists ", user = tokenString });
                }
                else
                {
                    // User doesn't exist, create a new user
                    var newUser = new Student
                    {
                        Name = userInfo.Name,
                        Email = userInfo.Email,
                        Password = ""
                    };

                    var result = _applicationDbContext.Add(newUser);
                    await _applicationDbContext.SaveChangesAsync();

                    var tokenString = GenerateToken(newUser.Name, newUser.Email, newUser.Id);
                    return Ok(new { message = "Login successful", user = tokenString });
                }
            }

            return BadRequest("Invalid Google access token");
        }

        private async Task<GoogleUserModel> ValidateGoogleAccessToken(string idToken)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var googleResponse = await httpClient.GetStringAsync($"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={idToken}");

                var userInfo = JsonConvert.DeserializeObject<GoogleUserModel>(googleResponse);
                return userInfo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Define a model for Google user information
  


        // GET: api/<StudentController>
       // [Authorize]
        [HttpGet]
        [Route("api/student")]
        public IActionResult Get(int id)
        {
            //var students = _applicationDbContext.Students.Where(x => x.Id== id);

            // var students = _applicationDbContext.Students.ToList();
            var students = _applicationDbContext.Students.IgnoreQueryFilters().ToList();
            
            return Ok(students);
        }



        [HttpPost]
        [Route("api/student")]
        public IActionResult Post(string name)
        {
            var student = new Student();
            student.Name = name;
                _applicationDbContext.Students.Add(student);
                _applicationDbContext.SaveChanges();

            return Ok("inserted");
        }

    }
}
