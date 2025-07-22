namespace Auth_with_JWT.Models
{
	public class TokenResponseDTO
	{
		public string AccessToken { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
		public int RoleId { get; set; } // Add RoleId
		public bool Status { get; set; } // Add Status
	}
}
