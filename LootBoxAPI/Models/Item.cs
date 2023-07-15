using RandomBoxAPI.Models;
using System.Text.Json.Serialization;

namespace LootBoxAPI.Models
{
    public class Item
    {
        public enum Tier
        {
            common,
            rare,
            super_rare,
            ultra_rare
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Tier Rarity { get; set; } = Tier.common;
        public float Price { get; set; } = 0;
        public byte[]? ImageData { get; set; }
        [JsonIgnore]
        public Inventory Inventory { get; set; }
        [JsonIgnore]
        public BoxItem  BoxItem { get; set; }
        public string Discriminator { get; set; } = "Item";
    }
}
