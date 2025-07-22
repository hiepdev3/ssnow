namespace Auth_with_JWT.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string? VoucherCode { get; set; }
        public Voucher? Voucher { get; set; }

        public int FieldId { get; set; }
        public Field Field { get; set; } = null!;

        public double DiscountAmount { get; set; }
        public double FinalPrice { get; set; }
        public DateTime PaidAt { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public int Status { get; set; }
        // Thêm hai trường mới
        public DateTime BookingDate { get; set; }
        public DateTime TimeEnd { get; set; }
    }
}