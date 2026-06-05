namespace LabScheduler.Web.Models
{
    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string Name { get; set; } = "";
        public string Username { get; set; } = "";
        public string Role { get; set; } = "";
    }

    public class AuthState
    {
        public string Token { get; set; } = "";
        public string Name { get; set; } = "";
        public string Username { get; set; } = "";
        public string Role { get; set; } = "";
        public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
        public bool IsAdmin => Role == "Admin";
    }
}
