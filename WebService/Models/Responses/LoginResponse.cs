using Newtonsoft.Json;

namespace WebService
{
    public class LoginResponse
    {
        public string Username { get; set; }
        public string Role { get; set; }
        [JsonIgnore]
        public Token Token { get; set; }
    }
}