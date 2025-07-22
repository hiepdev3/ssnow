namespace Auth_with_JWT.Entities
{
    public class MembershipTier
    {
        public int Id { get; set; } // Primary Key
        public string TierName { get; set; } = string.Empty; // Name of the tier
        public decimal MinTotalPayment { get; set; } // Minimum total payment required for this tier

        // Navigation property for related users
        public ICollection<User> Users { get; set; } = new List<User>();


    }
}
