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
    [Authorize(Roles = "Admin")] // Ensure only Admins with valid JWT tokens can access
    public class AdminController : ControllerBase
    {
        private readonly MyDbContext _context;

        public AdminController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Admin/getAllUsers
        [HttpGet("getAllUsers")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role) // Include Role for each user
                .Include(u => u.Tier) // Include MembershipTier for each user
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.FullName,
                    Role = u.Role.Name,
                    Tier = u.Tier.TierName,
                    u.TotalAmount,
                    u.Status,
                    u.CartCount
                })
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<object>>
            (
              200,
               "Successfully retrieved all users",
                users
            ));
        }

        // GET: api/Admin/getAllRoles
        [HttpGet("getAllRoles")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetAllRoles()
        {
            var roles = await _context.Roles
                .Select(r => new
                {
                    r.Id,
                    r.Name
                })
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<object>>
            (
                200,
               "Successfully retrieved all roles",
                roles
            ));
        }

        // GET: api/Admin/getAllMembershipTiers
        [HttpGet("getAllMembershipTiers")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetAllMembershipTiers()
        {
            var membershipTiers = await _context.MembershipTiers
                .Select(t => new
                {
                    t.Id,
                    t.TierName,
                    t.MinTotalPayment
                })
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<object>>
            (
               200,
                "Successfully retrieved all membership tiers",
                membershipTiers
            ));
        }

        // POST: api/Admin/addMembershipTiers
        [HttpPost("addMembershipTiers")]
        public async Task<ActionResult<ApiResponse<MembershipTierRequest>>> AddMembershipTiers([FromBody] MembershipTierRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.TierName))
                return BadRequest(new ApiResponse<MembershipTier>(400, "TierName is required", null));

            var exists = await _context.MembershipTiers.AnyAsync(t => t.TierName == model.TierName);
            if (exists)
                return Conflict(new ApiResponse<MembershipTier>(409, "TierName already exists", null));

            var tier = new MembershipTier
            {
                TierName = model.TierName,
                MinTotalPayment = model.MinTotalPayment
            };
            _context.MembershipTiers.Add(tier);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<MembershipTier>(200, "Membership tier added successfully", tier));
        }

        // PUT: api/Admin/editMembershipTiers/{id}
        [HttpPut("editMembershipTiers")]
        public async Task<ActionResult<ApiResponse<MembershipTier>>> EditMembershipTiers( [FromBody] MembershipTierRequest model)
        {
            var tier = await _context.MembershipTiers.FindAsync(model.Id);
            if (tier == null)
                return NotFound(new ApiResponse<MembershipTier>(404, "Membership tier not found", null));

            if (!string.IsNullOrWhiteSpace(model.TierName))
                tier.TierName = model.TierName;
            tier.MinTotalPayment = model.MinTotalPayment;

            await _context.SaveChangesAsync();
            return Ok(new ApiResponse<MembershipTier>(200, "Membership tier updated successfully", tier));
        }

        // PATCH: api/Admin/changeStatusAccounts/{userId}
        [HttpPatch("changeStatusAccounts/{userId}")]
        public async Task<ActionResult<ApiResponse<User>>> ChangeStatusAccounts(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new ApiResponse<User>(404, "User not found", null));

            user.Status = !user.Status;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<User>(200, "User status changed successfully", user));
        }
    }
}