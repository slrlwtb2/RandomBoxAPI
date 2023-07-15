using LootBoxAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RandomBoxAPI.Models
{
    public class BoxItem
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Box")]
        public int BoxId { get; set; }
        [ForeignKey("Item")]
        public int ItemId { get; set; }
        [JsonIgnore]
        public Item Item { get; set; }
    }
}
