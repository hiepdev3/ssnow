using Microsoft.AspNetCore.Identity;
using Auth_with_JWT.Entities;

namespace Auth_with_JWT.Helpers
{
    public static class PasswordHelper
    {
        // Hàm mã hóa password
        public static string HashPassword(string password)
        {
            var hasher = new PasswordHasher<User>();
            return hasher.HashPassword(new User(), password);
        }

        // Hàm kiểm tra password với hash
        public static bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(new User(), hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}