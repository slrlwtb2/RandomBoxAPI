using LootBoxAPI.Data;
using LootBoxAPI.DTO;
using LootBoxAPI.Models;
using LootBoxAPI.Repository.Interfaces;
using LootBoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LootBoxAPI.Repository
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;
        public InventoryRepository(IItemRepository itemListRepository,
            IUserRepository userRepository,
            ApplicationDbContext context)
        {
            _context = context;
            _itemRepository = itemListRepository;
            _userRepository= userRepository;
        }
        public async Task RemoveItemInInventroy(int itemId, int userId) //D
        {
            if (await _userRepository.UserExist(userId) && await _itemRepository.ItemExist(itemId))
            {
                if (await _context.Inventories.AnyAsync(inv => inv.UserId == userId && inv.ItemId == itemId))
                {
                    var originalInv = await _context.Inventories.FirstOrDefaultAsync(inv => inv.ItemId == itemId && inv.UserId == userId);
                    var updateInv = originalInv;
                    if (originalInv.Quantity > 1)
                    {
                        updateInv.Quantity -= 1;
                        await UpdateInventory(originalInv, updateInv);
                    }
                    else
                    {
                        _context.Inventories.Remove(originalInv);
                    } 
                }
            }
        }

        public async Task<List<InventoryDTO>> GetInventoryItemListByUserIdAsync(int id) //R
        {
            var inv = await _context.Inventories.Where(inv => inv.UserId == id && inv.Quantity>0).OrderBy(inv => inv.ItemId).ToListAsync();
            var inventoryDTOList = inv.Select(i => new InventoryDTO
            {
                Username = _userRepository.GetUsername(i.UserId),
                ItemId= i.ItemId,
                ItemName = _itemRepository.GetItemName(i.ItemId),
                Quantity = i.Quantity
            }).ToList();

            return inventoryDTOList;
        }

        public async Task<List<InventoryDTO>> GetInventories() //R
        {
            return await _context.Inventories.Select(inv => new InventoryDTO {
                
            } ).ToListAsync();
        }


        public async Task<bool> InventoryExist(int id) //R
        {
            var inv = await _context.Inventories.FindAsync(id);
            return inv!= null;
        }

        public async void Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Inventory> Add(Inventory inventory)
        {
            var addedInventory = await _context.Inventories.AddAsync(inventory);
            return addedInventory.Entity;
        }
        public async Task UpdateInventory(Inventory original,Inventory update)
        {
            var existingItem = await GetInventorybyUserId(original.Id);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(update);
                //_context.Entry(existingItem).State = EntityState.Modified;
            }

        }
        public async Task<Inventory> AddOrUpdate(Inventory inventory)
        {
            if (inventory.Id == 0)
            {
                var addedInventory = await _context.Inventories.AddAsync(inventory);
                return addedInventory.Entity;
            }
            else
            {
                _context.Inventories.Update(inventory);
                return inventory;
            }
        }

        public async Task<Inventory?> GetInventorybyUserId(int id)
        {
            var inv = await _context.Inventories.FirstOrDefaultAsync(x => x.Id == id);
            return inv;
        }
    }
}
