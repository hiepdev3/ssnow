namespace Auth_with_JWT.Models
{
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}