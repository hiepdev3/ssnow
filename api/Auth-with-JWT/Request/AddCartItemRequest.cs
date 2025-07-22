namespace Auth_with_JWT.Request
{
    public class AddCartItemRequest
    {
        public int FieldId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime TimeEnd { get; set; }
        public int Status { get; set; }
    }
}