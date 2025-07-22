namespace Auth_with_JWT.Request
{
    public class PaymentRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string VoucherCode { get; set; } = string.Empty;
        public int FieldId { get; set; }
        public double DiscountAmount { get; set; }
        public double FinalPrice { get; set; }
        public DateTime PaidAt { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime TimeEnd { get; set; }
    }
}