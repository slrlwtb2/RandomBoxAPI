using LootBoxAPI.DTO;
using LootBoxAPI.Models;

namespace LootBoxAPI.Repository.Interfaces
{
    public interface IInventoryRepository
    {

        Task<Inventory?> GetInventorybyUserId(int id); //R
        Task<List<InventoryDTO>> GetInventories(); //R
        Task<bool> InventoryExist(int id); //R
        Task RemoveItemInInventroy(int itemId, int userId); // D
        Task<List<InventoryDTO>> GetInventoryItemListByUserIdAsync(int id); // R
        Task<Inventory> Add(Inventory inventory); //C
        void Save(); // save change
        Task UpdateInventory(Inventory inventory,Inventory update);
        Task<Inventory> AddOrUpdate(Inventory inventory);
    }
}
