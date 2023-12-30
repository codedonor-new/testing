using Newtonsoft.Json;
using System;

namespace testing
{
    public class Student : CommanEntity, ISoftDelete
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; } = false;
        //hiiii
    }

    public class FacebookLoginModel
    {
        public string AuthToken { get; set; }
    }

    public class FacebookUserModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AuthToken { get; set; }
    }

    public class GoogleLoginModel
    {
        public string IdToken { get; set; }

       
    }
    public class GoogleUserModel
    {
        [JsonProperty("sub")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("picture")]
        public string PhotoUrl { get; set; }

    }
}

