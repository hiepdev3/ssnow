using Auth_with_JWT.Data;
using Auth_with_JWT.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Auth_with_JWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarkController : ControllerBase
    {
        private readonly MyDbContext _context;

        public MarkController(MyDbContext context)
        {
            _context = context;
        }

        public class AddMarkRequest
        {
            public int NumMark { get; set; }
            public double Distance { get; set; }
        }

        [HttpPost("addMark")]
        public async Task<IActionResult> AddMark([FromBody] AddMarkRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            var mark = new Mark
            {
                NumMart = request.NumMark,
                Distance = request.Distance
            };

            _context.Marks.Add(mark);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Data = mark
            });
        }

        [HttpGet("getMarkHighest")]
        public IActionResult GetMarkHighest()
        {
            var marks = _context.Marks.ToList();
            if (!marks.Any())
                return NotFound("No marks found.");

            var maxNumMart = marks.Max(m => m.NumMart);
            var maxDistance = marks.Max(m => m.Distance);

            var markMaxNumMart = marks.FirstOrDefault(m => m.NumMart == maxNumMart);
            var markMaxDistance = marks.FirstOrDefault(m => m.Distance == maxDistance);

            // If both highest NumMart and Distance are in the same record, return one object
            if (markMaxNumMart != null && markMaxDistance != null && markMaxNumMart.Id == markMaxDistance.Id)
            {
                return Ok(new
                {
                    Success = true,
                    Data = markMaxNumMart
                });
            }
            else
            {
                // Return a list of two objects (distinct if needed)
                var result = new List<Mark>();
                if (markMaxNumMart != null) result.Add(markMaxNumMart);
                if (markMaxDistance != null && markMaxDistance.Id != markMaxNumMart?.Id) result.Add(markMaxDistance);

                return Ok(new
                {
                    Success = true,
                    Data = result
                });
            }
        }
    }
}