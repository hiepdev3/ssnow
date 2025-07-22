using System.Text.Json.Serialization;

namespace Auth_with_JWT.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [JsonIgnore] // ⚠️ Chặn vòng lặp khi serialize JSON
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}