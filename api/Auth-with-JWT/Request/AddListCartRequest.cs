using System.Collections.Generic;

namespace Auth_with_JWT.Request
{
    public class AddListCartRequest
    {
        public int UserId { get; set; }
        public List<CartResponse> Carts { get; set; } = new();
    }
}