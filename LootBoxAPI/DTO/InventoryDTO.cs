namespace LootBoxAPI.DTO
{
    public class InventoryDTO
    {
        public string Username { get; set; }
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public string Rarity { get; set; }
        public float  Price { get; set; }
        public int Quantity { get; set; }
    }
}
