using System.Net.Http.Headers;
using System.Text.Json;
using Auth_with_JWT.Data;
using Auth_with_JWT.Entities;
using Auth_with_JWT.Helpers;
using Auth_with_JWT.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Auth_with_JWT.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class PublicController : ControllerBase
    {
        private readonly MyDbContext _context;

        public PublicController(MyDbContext context)
        {
            _context = context;
        }

        // POST: api/Public/listFieldForUser
        [HttpPost("listFieldForUser")]
        [AllowAnonymous] // Allow access without authentication
        public async Task<IActionResult> GetTop3Fields()
        {
            // Fetch top 2 fields with the highest price
            var top2Fields = await _context.Fields
                .Where(f => f.Status == 1)
                .OrderByDescending(f => f.Price)
                .Take(2)
                .ToListAsync();

            // Fetch the field with the lowest price (not in top 2)
            var lowestField = await _context.Fields
                .Where(f => f.Status == 1 && !top2Fields.Select(t => t.Id).Contains(f.Id))
                .OrderBy(f => f.Price)
                .FirstOrDefaultAsync();

            // Prepare the result
            var result = top2Fields.Select(f => new
            {
                f.Id,
                f.Name,
                f.Price,
                f.Province,
                f.District,
                f.Ward,
                f.Image
            }).ToList();

            if (lowestField != null)
            {
                result.Add(new
                {
                    lowestField.Id,
                    lowestField.Name,
                    lowestField.Price,
                    lowestField.Province,
                    lowestField.District,
                    lowestField.Ward,
                    lowestField.Image
                });
            }

            return Ok(new ApiResponse<object>(
                200,
                "Successfully retrieved top fields",
                result
            ));
        }

        // POST: api/Public/listAllFieldForUser
        [HttpPost("listAllFieldForUser")]
        [AllowAnonymous] // Allow access without authentication
        public async Task<IActionResult> ListAllFieldForUser()
        {
            var fields = await _context.Fields
                .Where(f => f.Status == 1)
                .OrderByDescending(f => f.Price)
                .Select(f => new
                {
                    f.Id,
                    f.Name,
                    f.Type,
                    f.Size,
                    f.Price,
                    f.Province,
                    f.District,
                    f.Ward,
                    f.SpecificAddress,
                    f.Description,
                    f.Image
                })
                .ToListAsync();

            return Ok(new ApiResponse<object>(
                200,
                "Successfully retrieved all fields",
                fields
            ));
        }


        [HttpPost("available-fields")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Field>>> GetAvailableFields([FromBody] FieldAvailabilityRequestcs request)
        {
            var startDateTime = request.Date.Date.Add(request.StartTime);
            var endDateTime = startDateTime.AddMinutes(request.Duration);

            // Lấy tất cả các sân
            var allFields = await _context.Fields.ToListAsync();

            // Lấy các payment đã thanh toán và trùng khung giờ
            var paidPayments = await _context.Payments
                .Where(p => p.Status == 1 &&
                            p.BookingDate < endDateTime &&
                            p.TimeEnd > startDateTime)
                .Select(p => p.FieldId)
                .ToListAsync();

            // Loại bỏ các sân đã được thanh toán trong khung giờ đó
            var availableFields = allFields
                .Where(f => !paidPayments.Contains(f.Id))
                .ToList();

            return Ok(new ApiResponse<IEnumerable<Field>>(200, "Success", availableFields));
        }

        [HttpPost("login-google")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.AccessToken))
                return BadRequest(new ApiResponse<object>(400, "Access token is required", null));

            // Check for non-ASCII or invalid characters
            if (request.AccessToken.Any(c => c > 127))
                return BadRequest(new ApiResponse<object>(400, "Access token contains invalid characters", null));

            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.AccessToken.Trim());

                var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
                if (!response.IsSuccessStatusCode)
                    return BadRequest(new ApiResponse<object>(400, "Invalid Google access token", null));

                var content = await response.Content.ReadAsStringAsync();
                var googleUser = JsonSerializer.Deserialize<GoogleUserInfocs>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (googleUser == null || string.IsNullOrEmpty(googleUser.Email))
                    return BadRequest(new ApiResponse<object>(400, "Failed to get user info from Google", null));

                // Kiểm tra user đã tồn tại chưa, nếu chưa thì tạo mới
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == googleUser.Email);
                if (user == null)
                {
                    user = new User
                    {
                        Email = googleUser.Email,
                        FullName = googleUser.Name ?? "",
                        Status = true,
                        RoleId = 3, // Gán role mặc định cho user mới (ví dụ: 2 = user)
                        TierId = 1  // Gán tier mặc định nếu có
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                // Ở đây bạn có thể sinh JWT token cho user nếu muốn
                // var token = GenerateJwtToken(user);

                return Ok(new ApiResponse<object>(200, "Login with Google successful", new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    // Token = token
                }));
            }
            catch (Exception ex)
            {
                // Log the exception (for debugging)
                Console.WriteLine("Exception in login-google: " + ex.Message);
                return StatusCode(500, new ApiResponse<object>(500, "Internal server error: " + ex.Message, null));
            }
        }
    }
}
