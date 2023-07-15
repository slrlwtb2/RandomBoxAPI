using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LootBoxAPI.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("ItemList")]
        
        public int ItemId { get; set; }
        public int Quantity { get; set; }

       
        
        
        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public Item? Item { get; set; }
    }

}
