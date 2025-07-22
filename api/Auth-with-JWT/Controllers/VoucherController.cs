using Auth_with_JWT.Data;
using Auth_with_JWT.Entities;
using Auth_with_JWT.Helpers;
using Auth_with_JWT.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Auth_with_JWT.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [ApiExplorerSettings(GroupName = "voucher")]
    public class VoucherController : Controller
    {
        private readonly MyDbContext _context;

        public VoucherController(MyDbContext context)
        {
            _context = context;
        }

        
        // POST: api/voucher/addvoucher
        [HttpPost("addvoucher")]
        public async Task<ActionResult<Voucher>> Add([FromBody] VoucherRequestAdd voucheradd)
        {
            try
            {
                var voucher = new Voucher
                {
                    Code = voucheradd.Code,
                    DiscountPercent = voucheradd.DiscountPercent,
                    MembershipTierId = voucheradd.MembershipTierId,
                    FieldId = voucheradd.FieldId,
                    UserId = voucheradd.UserId,
                    StartDate = voucheradd.StartDate,
                    EndDate = voucheradd.EndDate,
                    Quantity = voucheradd.Quantity,
                    Status = voucheradd.Status
                };

                _context.Vouchers.Add(voucher);
                await _context.SaveChangesAsync();
                return Ok(new ApiResponse<Voucher>(200, "successfull", voucher));
            }
            catch
            {
                return BadRequest(new ApiResponse<Voucher>(400, "Failed to add voucher", null));
            }
        }
        // POST: api/voucher/getallvoucherbyuserid
        [HttpPost("getallvoucherbyuserid")]
            public async Task<ActionResult<IEnumerable<object>>> GetAllVoucherByUserId([FromBody] UserIdRequest request)
            {
                if (!int.TryParse(request.UserId, out int parsedUserId))
                {
                    return BadRequest("Invalid userId format.");
                }

                var now = DateTime.Now;

                // Lấy tất cả voucher của user
                var vouchers = await _context.Vouchers
                    .Where(v => v.UserId == parsedUserId)
                    .Include(v => v.Field)
                    .OrderByDescending(v => v.Id)
                    .ToListAsync();

                // Cập nhật trạng thái nếu hết hạn
                bool hasChanged = false;
                foreach (var v in vouchers)
                {
                    if ((v.Status == 0 || v.Status == 1 || v.Status == 2) && v.EndDate < now)
                    {
                        v.Status = 3; // Hết hạn
                        hasChanged = true;
                    }
                }

                if (hasChanged)
                {
                    await _context.SaveChangesAsync();
                }

                // Trả về kết quả
                var result = vouchers.Select(v => new
                {
                    v.Id,
                    v.Code,
                    v.DiscountPercent,
                    v.MembershipTierId,
                    v.FieldId,
                    FieldName = v.Field != null ? v.Field.Name : null,
                    v.UserId,
                    v.StartDate,
                    v.EndDate,
                    v.Quantity,
                    v.Status,
                }).ToList();

                return Ok(result);
            }

        [HttpPut("updatevoucher")]
        public async Task<ActionResult<Voucher>> UpdateVoucher([FromBody] VoucherRequestAdd voucherUpdate)
        {
            var existingVoucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == voucherUpdate.Code);
            if (existingVoucher == null)
            {
                return NotFound(new ApiResponse<Voucher>(404, "Voucher not found", null));
            }

            // Update fields
            existingVoucher.DiscountPercent = voucherUpdate.DiscountPercent;
            existingVoucher.MembershipTierId = voucherUpdate.MembershipTierId;
            existingVoucher.FieldId = voucherUpdate.FieldId;
            existingVoucher.UserId = voucherUpdate.UserId;
            existingVoucher.StartDate = voucherUpdate.StartDate;
            existingVoucher.EndDate = voucherUpdate.EndDate;
            existingVoucher.Quantity = voucherUpdate.Quantity;
            existingVoucher.Status = voucherUpdate.Status;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new ApiResponse<Voucher>(200, "Update successful", existingVoucher));
            }
            catch
            {
                return BadRequest(new ApiResponse<Voucher>(400, "Failed to update voucher", null));
            }
        }
    }
}
