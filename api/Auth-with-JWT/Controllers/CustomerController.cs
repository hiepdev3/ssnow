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
    [Authorize(Roles = "Customer")]
    public class CustomerController : ControllerBase
    {
        private readonly MyDbContext _context;

        public CustomerController(MyDbContext context)
        {
            _context = context;
        }

        // POST: api/Customer/addListCart
        [HttpPost("addListCart")]
        public async Task<ActionResult<ApiResponse<object>>> AddListCart([FromBody] AddListCartRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return NotFound(new ApiResponse<string>(404, "User not found", null));

            // Lấy tất cả FieldId đã có trong Cart của user này
            var existingFieldIds = await _context.Carts
                .Where(c => c.UserId == request.UserId)
                .Select(c => c.FieldId)
                .ToListAsync();

            var cartsToAdd = new List<Cart>();

            foreach (var item in request.Carts)
            {
                // Nếu FieldId đã tồn tại thì bỏ qua
                if (existingFieldIds.Contains(item.Id))
                    continue;

                // Ghép date + timeSlots thành DateTime cho BookingDate
                var bookingDate = DateTime.Parse($"{item.Date} {item.TimeSlots}");
                var timeEnd = bookingDate.AddHours(item.Duration);

                var cart = new Cart
                {
                    UserId = request.UserId,
                    FieldId = item.Id,
                    BookingDate = bookingDate,
                    TimeEnd = timeEnd,
                    Status = 0,
                    Duration = item.Duration,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                cartsToAdd.Add(cart);
            }

            if (cartsToAdd.Count > 0)
            {
                _context.Carts.AddRange(cartsToAdd);
                await _context.SaveChangesAsync();
            }


            return Ok(new ApiResponse<object>(200, "Carts added successfully", null));
        }

        // POST: api/Customer/addTheCart
        [HttpPost("addTheCart")]
        public async Task<ActionResult<ApiResponse<object>>> AddTheCart([FromBody] CartResponse request, [FromQuery] int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new ApiResponse<string>(404, "User not found", null));

            // Check if the cart with this FieldId already exists for this user
            bool exists = await _context.Carts
                .AnyAsync(c => c.UserId == userId && c.FieldId == request.Id);

            if (exists)
                return BadRequest(new ApiResponse<string>(400, "Cart already exists for this field and user", null));

            // Parse BookingDate and TimeEnd
            var bookingDate = DateTime.Parse($"{request.Date} {request.TimeSlots}");
            var timeEnd = bookingDate.AddHours(request.Duration);

            var cart = new Cart
            {
                UserId = userId,
                FieldId = request.Id,
                BookingDate = bookingDate,
                TimeEnd = timeEnd,
                Duration = request.Duration,
                Status = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>(200, "Cart added successfully", null));
        }

        // GET: api/Customer/viewAllCart?userId=2
        [HttpGet("viewAllCart")]
        public async Task<ActionResult<ApiResponse<List<CartResponse>>>> ViewAllCart([FromQuery] int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new ApiResponse<string>(404, "User not found", null));

            var userCarts = await _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Field)
                .ToListAsync();

            var cartResponses = userCarts.Select(c => new CartResponse
            {
                Id = c.FieldId,
                Name = c.Field?.Name ?? "",
                Location = $"{c.Field?.Ward}, {c.Field?.District}, {c.Field?.Province}",
                Rating = 4.5,
                Features = new List<string> { "Floodlights", "Spectator Area", "Changing Rooms" },
                Availability = "High",
                TimeSlots = c.BookingDate.ToString("HH:mm"),
                PricePerHour = c.Field?.Price ?? 0,
                Image = c.Field?.Image ?? "",
                Date = c.BookingDate.ToString("yyyy-MM-dd"),
                Duration = c.Duration
            }).ToList();

            return Ok(new ApiResponse<List<CartResponse>>(200, "Success", cartResponses));
        }
    }
}