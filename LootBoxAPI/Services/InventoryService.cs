using LootBoxAPI.Data;
using LootBoxAPI.DTO;
using LootBoxAPI.Models;
using LootBoxAPI.Repository;
using LootBoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using RandomBoxAPI.Models;
using RandomBoxAPI.Repository.Interfaces;
using static LootBoxAPI.Models.Item;

namespace LootBoxAPI.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IRepository<Inventory, int> _inventoryRepository;
        private readonly IRepository<Item, int> _itemRepository;
        private readonly IRepository<User, int> _userRepository;
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IItemService _itemService;

        public InventoryService(
            IRepository<Inventory,int> inventoryRepository,
            IRepository<Item, int> itemListRepository,
            IRepository<User, int> userRepository,
            ApplicationDbContext context,
            IUserService userService,
            IItemService itemService)
        {
            _inventoryRepository = inventoryRepository;
            _itemRepository = itemListRepository;
            _userRepository = userRepository;
            _context = context;
            _userService = userService;
            _itemService = itemService;
        }

        public async Task<Inventory> AddItemtoInventory(int itemId, int userId, int amount)
        {
            if (await CheckUserAndItemExist(userId,itemId))
            {
                Inventory Inventory = new Inventory()
                {
                    UserId = userId,
                    ItemId = itemId,
                    Quantity = amount
                };

                await _inventoryRepository.Add(Inventory);
                return Inventory; 
            }
            throw new ArgumentException("User or Item do not exist");
        }
        public async Task<Inventory> UpdateItemtoInventory(int itemId, int userId, int amount)
        {
            if (await CheckUserAndItemExist(userId,itemId))
            {
                var inventory = await _context.Inventories.FirstOrDefaultAsync(inv => inv.UserId == userId && inv.ItemId == itemId);
                if (inventory != null)
                {
                    inventory.Quantity += amount;
                    _inventoryRepository.Update(inventory);
                    return inventory;
                }
            }
            throw new ArgumentException("User or Item do not exist");
        }
        public async Task<Inventory> SellItem(int itemId, int userId, int amount)
        {
            if (await CheckUserAndItemExist(userId,itemId))
            {
                var inventory = await _context.Inventories.FirstOrDefaultAsync(inv => inv.UserId == userId && inv.ItemId == itemId);
                if (inventory != null)
                {
                    var quantity = await GetQuantity(itemId, userId);
                    var sellValue = await _itemRepository.GetById(itemId);
                    if (quantity - amount < 0 || amount > quantity) { throw new ArgumentException("insufficient amount of item's quantity", nameof(quantity)); }
                    inventory.Quantity -= amount;
                    var user = await _userRepository.GetById(userId);
                    user.Balance += sellValue.Price * amount;
                    return inventory;  
                }
            }
            throw new ArgumentException("User or Item do not exist");
        }
        public async Task<Inventory> DiscardItem(int itemId, int userId, int amount)
        {
            if (await CheckUserAndItemExist(userId, itemId))
            {
                var inventory = await _context.Inventories.FirstOrDefaultAsync(inv => inv.UserId == userId && inv.ItemId == itemId);
                if (inventory !=null)
                {
                    var quantity = await GetQuantity(itemId, userId);
                    if (quantity - amount < 0 || amount > quantity) { throw new ArgumentException("insufficient amount of item's quantity", nameof(quantity)); }
                    inventory.Quantity -= amount;
                    return inventory; 
                }
            }
            throw new ArgumentException("User or Item do not exist");
        }
        public async Task<bool> AlreadyHaveItem(int itemId, int userId)
        {
            var containItem = await _context.Inventories.AnyAsync(inv => inv.UserId == userId && inv.ItemId == itemId);
            return containItem;
        }
        public async Task<int> GetQuantity(int itemId, int userId)
        {
            var containItem = await _context.Inventories.FirstOrDefaultAsync(inv => inv.UserId == userId && inv.ItemId == itemId);
            if (containItem != null)
            {
                return containItem.Quantity; 
            }
            return 0;
        }
        public async Task<Item> OpenBox(List<BoxItem> items)
        {
            var rarity = GetRandomRarity();

            List<Item> listOfItems = new List<Item>();

            foreach (var item in items)
            {
                if (await _itemRepository.Exist(item.ItemId))
                {
                    var randomItem = await _itemRepository.GetById(item.ItemId);

                    if (randomItem.Rarity == rarity)
                    {
                        listOfItems.Add(randomItem);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    throw new ArgumentException($"The 'itemid' {item.ItemId} does not exist");
                }
            }
            if (listOfItems.Count > 0)
            {
                Random rnd = new Random();
                int index = rnd.Next(listOfItems.Count);
                return listOfItems[index];
            }
            throw new InvalidOperationException("No item with matching rarity found.");
        }
        public async Task<bool> CheckUserAndItemExist(int userId, int itemId)
        {
            var user = await _context.Users.FindAsync(userId);
            var item = await _context.Items.FindAsync(itemId);
            if (user != null && item != null) return true;
            return false;

        }
        public Tier GetRandomRarity()
        {
            Tier rarity;
            Random rnd = new Random();
            int randomNumber = rnd.Next(1, 101);

            if (randomNumber <= 50)
            {
                rarity = Tier.common;
                return rarity;
            }
            else if (randomNumber <= 60)
            {
                rarity = Tier.rare;
                return rarity;
            }
            else if (randomNumber <= 70)
            {
                rarity = Tier.super_rare;
                return rarity;
            }
            else
            {
                rarity = Tier.ultra_rare;
                return rarity;
            }
        }
        public async Task RemoveItemInInventroy(int itemId, int userId) 
        {
            if (await _userRepository.Exist(userId) && await _itemRepository.Exist(itemId))
            {
                if (await _context.Inventories.AnyAsync(inv => inv.UserId == userId && inv.ItemId == itemId))
                {
                    var inventory= await _context.Inventories.FirstOrDefaultAsync(inv => inv.ItemId == itemId && inv.UserId == userId);
                    if (inventory != null)
                    {
                        if (inventory.Quantity > 1)
                        {
                            inventory.Quantity -= 1;
                            _inventoryRepository.Update(inventory);
                        }
                        else
                        {
                            _context.Inventories.Remove(inventory);
                        } 
                    }
                }
            }
        }
        public async Task<List<InventoryDTO>> GetInventoryItemListByUserIdAsync(int id)
        {
            var inv = await _context.Inventories
                .Where(inv => inv.UserId == id && inv.Quantity > 0)
                .OrderBy(inv => inv.ItemId)
                .ToListAsync();

            var inventoryDTOList = new List<InventoryDTO>();

            foreach (var i in inv)
            {
                var username = await _userService.GetUsername(i.UserId);
                var rarity = _itemService.GetItemRarity(i.ItemId);
                var itemName = _itemService.GetItemName(i.ItemId);
                var price = _itemService.GetItemPrice(i.ItemId);

                inventoryDTOList.Add(new InventoryDTO
                {
                    Username = username,
                    ItemId = i.ItemId,
                    Rarity = rarity,
                    ItemName = itemName,
                    Price = price,
                    Quantity = i.Quantity
                });
            }

            return inventoryDTOList;
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
    }
}
