namespace Auth_with_JWT.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int FieldId { get; set; }
        public Field Field { get; set; } = null!;

        public DateTime BookingDate { get; set; }
        public DateTime TimeEnd { get; set; } // Thời gian kết thúc booking
        public int Duration { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}