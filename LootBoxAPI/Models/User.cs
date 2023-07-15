using System.ComponentModel.DataAnnotations;

namespace LootBoxAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];
        public float Balance { get; set; } = 300;
        public string Role { get; set; } = "User";
        public Inventory? Inventory { get; set; }
    }
}
