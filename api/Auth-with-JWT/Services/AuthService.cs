using Auth_with_JWT.Data;
using Auth_with_JWT.Entities;
using Auth_with_JWT.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth_with_JWT.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _context;

        public AuthService(IConfiguration configuration, MyDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> RegisterUserAsync(User user)
        {
            await _context.Users.AddAsync(user);        
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> RegisterAsync(UserDTO request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return null;
            var user = new User();

            user.Email = request.Email;
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<TokenResponseDTO> LoginAsync(UserDTO request)
        {
            User? user = await _context.Users
                .Include(u => u.Role) // Ensure Role is included
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null)
                return null;

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }
            var token = new TokenResponseDTO
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshToken(user)
            };

            return token;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role) // Include Role if needed
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role) // Include Role if needed
                .FirstOrDefaultAsync(u => u.Email == email);
        }


        public async Task<string> GenerateAndSaveRefreshToken(User user)
        {
            var randomnumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomnumber);
            var refreshToken = Convert.ToBase64String(randomnumber);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? string.Empty) // Use Role.Name instead of Role
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetValue<string>("Jwt:Key")!
            ));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            // Define expiresInMinutes with a default value or retrieve it from configuration
            int expiresInMinutes = _configuration.GetValue<int>("Jwt:ExpiresInMinutes", 30);

            var tokenDistributer = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDistributer);
        }

        public async Task<TokenResponseDTO?> RefreshTokenAsync(RefreshTokenRequestDTO request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
                return null;
            var token = new TokenResponseDTO
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshToken(user)
            };
            return token;
        }
    }
}

