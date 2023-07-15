using Microsoft.AspNetCore.Mvc;

namespace RandomBoxAPI.DTO
{
    public class ItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Rarity { get; set; }
        public float Price { get; set; }
        public bool ImageData { get; set; }
    }
}
