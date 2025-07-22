namespace Auth_with_JWT.Entities
{
    public class Voucher
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty; // Unique
        public int DiscountPercent { get; set; }
        public int MembershipTierId { get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
    }
}