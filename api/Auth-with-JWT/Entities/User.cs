namespace Auth_with_JWT.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        // Foreign key to Role
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiry { get; set; }

        // Additional fields
        public int TierId { get; set; }
        public MembershipTier Tier { get; set; } = null!; // Navigation property


        public decimal TotalAmount { get; set; }
        public string? PhoneNumber { get; set; } 
        public bool Status { get; set; }  // true = active, false = inactive
        public int CartCount { get; set; } // Default value will be 0

    }
}
