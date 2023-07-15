using LootBoxAPI.Data;
using LootBoxAPI.Models;
using LootBoxAPI.Repository;
using LootBoxAPI.Repository.Interfaces;
using LootBoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using RandomBoxAPI.Models;
using static LootBoxAPI.Models.Item;

namespace LootBoxAPI.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IItemRepository itemListRepository,
            IUserRepository userRepository,
            ApplicationDbContext context)
        {
            _inventoryRepository = inventoryRepository;
            _itemRepository = itemListRepository;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<Inventory> AddItemtoInventory(int itemId, int userId, int amount)
        {
            var user = await _userRepository.GetUserbyId(userId);
            if (user == null) { throw new ArgumentException("No 'user' Id.", nameof(userId)); }
            var item = await _itemRepository.ItemExist(itemId);
            if (item == false) { throw new ArgumentException("No 'Item' Id.", nameof(itemId)); }

            Inventory Inventory = new Inventory()
            {
                UserId = userId,
                ItemId = itemId,
                Quantity = amount
            };

            await _inventoryRepository.Add(Inventory);
            return Inventory;
        }

        public async Task<Inventory> UpdateItemtoInventory(int itemId, int userId, int amount)
        {
            var user = await _userRepository.GetUserbyId(userId);
            if (user == null) { throw new ArgumentException("No 'user' Id.", nameof(userId)); }
            var item = await _itemRepository.ItemExist(itemId);
            if (item == false) { throw new ArgumentException("No 'Item' Id.", nameof(itemId)); }

            var original = await _context.Inventories.FirstOrDefaultAsync(inv => inv.UserId == userId && inv.ItemId == itemId);
            var quantity = await GetQuantity(itemId, userId);

            Inventory Inventory = new Inventory()
            {
                Id = original.Id,
                UserId = userId,
                ItemId = itemId,
                Quantity = quantity + amount
            };

            await _inventoryRepository.UpdateInventory(original, Inventory);
            return Inventory;
        }

        public async Task<Inventory> SellItem(int itemId, int userId, int amount)
        {
            var user = await _userRepository.GetUserbyId(userId);
            if (user == null) { throw new ArgumentException("No 'user' Id.", nameof(userId)); }
            var item = await _itemRepository.ItemExist(itemId);
            if (item == false) { throw new ArgumentException("No 'Item' Id.", nameof(itemId)); }

            var original = await _context.Inventories.FirstOrDefaultAsync(inv => inv.UserId == userId && inv.ItemId == itemId);
            var quantity = await GetQuantity(itemId, userId);

            if (quantity - amount < 0 || amount > quantity) { throw new ArgumentException("insufficient amount of item's quantity", nameof(quantity)); }

            var sellValue = await _itemRepository.GetItembyIdAsync(itemId);

            Inventory Inventory = new Inventory()
            {
                Id = original.Id,
                UserId = userId,
                ItemId = itemId,
                Quantity = quantity - amount
            };

            var usertemp = await _userRepository.GetUserbyId(userId);
            usertemp.Balance += sellValue.Price * amount;

            await _inventoryRepository.UpdateInventory(original, Inventory);
            return Inventory;
        }

        public async Task<Inventory> DiscardItem(int itemId, int userId, int amount)
        {
            amount = 1;
            var user = await _userRepository.GetUserbyId(userId);
            if (user == null) { throw new ArgumentException("No 'user' Id.", nameof(userId)); }
            var item = await _itemRepository.ItemExist(itemId);
            if (item == false) { throw new ArgumentException("No 'Item' Id.", nameof(itemId)); }

            var original = await _context.Inventories.FirstOrDefaultAsync(inv => inv.UserId == userId && inv.ItemId == itemId);
            var quantity = await GetQuantity(itemId, userId);

            if (quantity - amount < 0 || amount > quantity) { throw new ArgumentException("insufficient amount of item's quantity", nameof(quantity)); }

            Inventory Inventory = new Inventory()
            {
                Id = original.Id,
                UserId = userId,
                ItemId = itemId,
                Quantity = quantity - amount
            };
            Console.WriteLine(Inventory);
            await _inventoryRepository.UpdateInventory(original, Inventory);
            return Inventory;
        }

        public async Task<bool> AlreadyHaveItem(int itemId, int userId)
        {
            var containItem = await _context.Inventories.AnyAsync(inv => inv.UserId == userId && inv.ItemId == itemId);
            return containItem;
        }

        public async Task<int> GetQuantity(int itemId, int userId)
        {
            var containItem = await _context.Inventories.FirstOrDefaultAsync(inv => inv.UserId == userId && inv.ItemId == itemId);
            return containItem.Quantity;
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
            else if (randomNumber <= 80)
            {
                rarity = Tier.rare;
                return rarity;
            }
            else if (randomNumber <= 95)
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
        public async Task<Item> OpenBox(List<BoxItem> items)
        {
            var rarity = GetRandomRarity();

            List<Item> listOfItems = new List<Item>();

            foreach (var item in items)
            {
                if (await _itemRepository.ItemExist(item.ItemId))
                {
                    var randomItem = await _itemRepository.GetItembyIdAsync(item.ItemId);

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
    }
}
