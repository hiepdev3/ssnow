namespace Auth_with_JWT.Request
{
    public class VoucherRequestAdd
    {
        public string Code { get; set; } = string.Empty;
        public int DiscountPercent { get; set; }
        public int MembershipTierId { get; set; }
        public int FieldId { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
    }
}