namespace Auth_with_JWT.Entities
{
    public class Field
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Size { get; set; }
        public double Price { get; set; }
        public string Province { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string SpecificAddress { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Description { get; set; } = string.Empty;
        public int UserId { get; set; } // Foreign key
        public User User { get; set; } = null!;
        public string Image { get; set; } = string.Empty;
    }
}