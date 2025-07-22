namespace Auth_with_JWT.Request
{
    public class FieldRequestAdd
    {
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int Size { get; set; }
        public double Price { get; set; }
        public string Province { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Ward { get; set; } = null!;
        public string SpecificAddress { get; set; } = null!;
        public int Status { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }
    }
}
