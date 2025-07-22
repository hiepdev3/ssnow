using Auth_with_JWT.Entities;
using Auth_with_JWT.Models;

namespace Auth_with_JWT.Services
{
	public interface IAuthService
	{
        Task<bool> IsEmailTakenAsync(string email);
        Task<User?> RegisterUserAsync(User user);

        Task<TokenResponseDTO?> LoginAsync(UserDTO request);
		Task<TokenResponseDTO?> RefreshTokenAsync(RefreshTokenRequestDTO request);
		Task<User?> RegisterAsync(UserDTO request);
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);

    }
}