using Auth_with_JWT.Data;
using Auth_with_JWT.Entities;
using Auth_with_JWT.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Auth_with_JWT.Request;
namespace Auth_with_JWT.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [ApiExplorerSettings(GroupName = "manager")]
    public class FieldController : Controller
    {
        private readonly MyDbContext _context;

        public FieldController(MyDbContext context)
        {
            _context = context;
        }
        // POST: api/field/getall
        [HttpGet("getall")]
        public async Task<ActionResult<IEnumerable<Field>>> GetAll()
        {
            var fields = await _context.Fields.ToListAsync();
            return Ok(new ApiResponse<IEnumerable<Field>>(200, "Success", fields));
        }

        // POST: api/field/addfield
        [HttpPost("addfield")]
        public async Task<ActionResult<Field>> Add([FromBody] FieldRequestAdd dto)
        {
            var field = new Field
            {
                Name = dto.Name,
                Type = dto.Type,
                Size = dto.Size,
                Price = dto.Price,
                Province = dto.Province,
                District = dto.District,
                Ward = dto.Ward,
                SpecificAddress = dto.SpecificAddress,
                Status = dto.Status,
                Description = dto.Description,
                UserId = dto.UserId
            };
            try
            {
                _context.Fields.Add(field);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<Field>(200, "successfull", null));
            }
            catch
            {
                return BadRequest(new ApiResponse<Field>(400, "Failed to add field", null));
            }
        }

        // POST: api/field/getallfieldbyUserid
        [HttpPost("getallfieldbyUserid")]
        public async Task<ActionResult<IEnumerable<Field>>> GetFieldsByUserId([FromBody] UserIdRequest request)
        {
            if (!int.TryParse(request.UserId, out int parsedUserId))
            {
                return BadRequest(new ApiResponse<IEnumerable<Field>>(400, "Invalid userId format.", null));
            }

            var fields = await _context.Fields
                .Where(f => f.UserId == parsedUserId)
                .ToListAsync();

            return Ok(new ApiResponse<IEnumerable<Field>>(200, "Success", fields));

        }
    }
}
