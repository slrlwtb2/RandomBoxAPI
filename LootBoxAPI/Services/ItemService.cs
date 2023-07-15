using LootBoxAPI.Data;
using LootBoxAPI.Models;
using LootBoxAPI.Repository.Interfaces;
using LootBoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using RandomBoxAPI.Models;
using System;
using static LootBoxAPI.Models.Item;

namespace LootBoxAPI.Services
{
    public class ItemService : IItemService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IItemRepository _itemsRepository;
        private readonly ApplicationDbContext _context;
        public ItemService(IInventoryRepository inventoryRepository,IItemRepository itemListRepository,ApplicationDbContext context)
        {
            _inventoryRepository= inventoryRepository;
            _itemsRepository= itemListRepository;
            _context = context;
        }

        public Item CreateBox(string name, int rarity, float price)
        {
            CreateItemValidation(name, rarity, price);
            Item.Tier tier = (Item.Tier)rarity;
            Item item = new Item
            {
                Name = name,
                Rarity = tier,
                Price = price,
                Discriminator="Box"
            };
            return item;
        }

        public Item CreateItem(string name, int rarity, float price)
        {
            CreateItemValidation(name, rarity, price);
            Item.Tier tier = (Item.Tier)rarity;
            Item item = new Item
            {
                Name = name,
                Rarity = tier,
                Price = price
            };
            return item;
        }
        public void CreateItemValidation(string name,int rarity,float price)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The 'name' parameter is required.", nameof(name));
            }

            if (!Enum.IsDefined(typeof(Item.Tier), rarity))
            {
                throw new ArgumentException("The 'rarity' parameter is not a valid value.", nameof(rarity));
            }

            if (price <= 0)
            {
                throw new ArgumentException("The 'price' parameter must be a positive value.", nameof(price));
            }
        }
        public async Task<bool> BoxAndItemExist(int itemid1, int itemid2)
        {
            var items = await _itemsRepository.ItemExist(itemid1) != false && await _itemsRepository.ItemExist(itemid2);
            return items;
        }
        public bool AreBox(Item box)
        {
            return box.Discriminator == "Box";
        }
        public bool AreItem(Item item)
        {
            return item.Discriminator == "Item";
        }
        public async Task<bool> AlreadyContainItemInBox(int boxid, int itemid)
        {
            return await _context.BoxItems.AnyAsync(b => b.BoxId == boxid && b.ItemId == itemid);
        }
        public BoxItem CreateBoxItemList(BoxItem boxItem,int boxid,int itemid)
        {
            boxItem = new BoxItem()
            {
                BoxId = boxid,
                ItemId = itemid
            };
            return boxItem;
        }

    }
}
