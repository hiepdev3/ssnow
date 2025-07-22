namespace Auth_with_JWT.Request
{
    public class MembershipTierRequest
    {
        public int Id { get; set; }
        public string TierName { get; set; } = string.Empty;
        public decimal MinTotalPayment { get; set; }

    }
}
