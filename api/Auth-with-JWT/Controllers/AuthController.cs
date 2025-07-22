using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Auth_with_JWT.Data;
using Auth_with_JWT.Entities;
using Auth_with_JWT.Helpers;
using Auth_with_JWT.Models;
using Auth_with_JWT.Request;
using Auth_with_JWT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Auth_with_JWT.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [ApiExplorerSettings(GroupName = "auth")] // Gán group cho Swagger UI
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }



        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Register([FromBody] RegisterRequest registerRequest)
        {
            // Check if email already exists
            if (await _authService.IsEmailTakenAsync(registerRequest.Email))
            {
              
                return BadRequest(new ApiResponse<User>(
                    400,
                    "User already exists",
                    null
                ));
            }

            // Map RegisterRequest to User
            var user = new User
            {
                Email = registerRequest.Email,
                PasswordHash = PasswordHelper.HashPassword(registerRequest.PasswordHash),
                FullName = registerRequest.FullName,
                RoleId = registerRequest.RoleName.ToLower() switch
                {
                    "customer" => 3,
                    "manager" => 2,
                    _ => 1 // Default RoleId if RoleName is not recognized
                },
                TierId = 1, // Default TierId
                TotalAmount = 0, // Default TotalAmount
                PhoneNumber = string.IsNullOrWhiteSpace(registerRequest.PhoneNumber) ? null : registerRequest.PhoneNumber,
                Status = true, // Default Status
                CartCount = 0
            };

            // Register the user
            var createdUser = await _authService.RegisterUserAsync(user);
            if (createdUser is null)
            {
                return BadRequest(new ApiResponse<User>(
                    400,
                    "Failed to register user",
                    null
                ));
            }

            return Ok(new ApiResponse<User>
            (
                200,
                 "User registered successfully",
                createdUser
            ));
        }

        // Đăng nhập - Cho phép truy cập không cần xác thực
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<TokenResponseDTO>>> Login([FromBody] UserDTO request)
        {
            var token = await _authService.LoginAsync(request);
            if (token is null)
            {
                return BadRequest(new ApiResponse<TokenResponseDTO>(
                     400,
                     "Wrong username or password",
                     null
                 ));
            }

            // Include RoleId and Status in the response
            var user = await _authService.GetUserByEmailAsync(request.Email);
            if (user is null)
            {
                return Ok(new ApiResponse<TokenResponseDTO>(400, "User not found", null));
            }

            token.RoleId = user.RoleId;
            token.Status = user.Status;

            return Ok(new ApiResponse<TokenResponseDTO>(200,"Login successful", token));
        }

        // Refresh token - Cho phép truy cập không cần xác thực
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDTO>> RefreshToken([FromBody] RefreshTokenRequestDTO request)
        {
            var token = await _authService.RefreshTokenAsync(request);
            if (token is null)
                return BadRequest("Invalid or expired token");
            return Ok(token);
        }

        // Endpoint yêu cầu xác thực
        [HttpGet("auth-endpoint")]
        [Authorize]
        public IActionResult AuthCheck()
        {
            return Ok("Authenticated");
        }

        // Endpoint yêu cầu quyền Admin
        [HttpGet("admin-endpoint")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminCheck()
        {
            return Ok("Admin authenticated");
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<User>>> Me()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Ok(new ApiResponse<User>(400, "Invalid token", null));
            }

            var user = await _authService.GetUserByIdAsync(int.Parse(userId));
            if (user is null)
            {
                return Ok(new ApiResponse<User>(404, "User not found", null));
            }

            return Ok(new ApiResponse<object>(
                200,
                "User information retrieved successfully",
                new
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = new { Id = user.Role.Id }, // Only include Role ID
                    RefreshToken = user.RefreshToken,
                    RefreshTokenExpiry = user.RefreshTokenExpiry,
                    TierId = user.TierId,
                    
                    TotalAmount = user.TotalAmount,
                    Status = user.Status , // Invert logic: 0 -> falase, 1 -> true
                    CartCount = user.CartCount
                }
            ));
        }


       
    }
}
