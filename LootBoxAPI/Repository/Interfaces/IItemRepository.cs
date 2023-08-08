using LootBoxAPI.Models;
using RandomBoxAPI.DTO;
using RandomBoxAPI.Models;

namespace LootBoxAPI.Repository.Interfaces
{
    public interface IItemRepository
    {

        Task<List<ItemDTO>> GetItemsAsync();
        Task<Item> GetItembyIdAsync(int id);
        Task<Item> GetItemsbyNameAsync(string name);
        Task InsertItem(Item item);
        Task<bool> ItemExist(int id);
        Task UpdateItem(Item original, Item update);
        void DeleteItemAsync(int id);
        void Save();
        Task<List<BoxItem>> GetItemInBox(int boxid);
        string GetItemName(int itemid);
        string GetItemRarity(int itemid);
        float GetItemPrice(int itemid);
    }
}
