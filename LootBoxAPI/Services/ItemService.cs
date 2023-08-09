using LootBoxAPI.Data;
using LootBoxAPI.Models;
using LootBoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using RandomBoxAPI.Models;
using RandomBoxAPI.Repository.Interfaces;
using System;
using static LootBoxAPI.Models.Item;

namespace LootBoxAPI.Services
{
    public class ItemService : IItemService
    {
        private readonly IRepository<Item,int> _itemsRepository;
        private readonly ApplicationDbContext _context;
        public ItemService(IRepository<Item,int> itemListRepository,ApplicationDbContext context)
        {
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
        public async Task<bool> BoxAndItemExist(int itemId, int boxId)
        {
            var box = await _itemsRepository.GetById(itemId);
            var item = await _itemsRepository.GetById(boxId);
            if (item != null && box != null)
            {
                if (AreItem(item) && AreBox(box)) return true; 
            }
            return false;
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
            var item = _itemsRepository.GetById(itemid);
            var box = _itemsRepository.GetById(boxid);
            if (item == null || box == null) return false;
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
        public async Task<List<BoxItem>> GetItemInBox(int boxid)
        {
            var items = await _context.BoxItems.Where(b => b.BoxId == boxid).ToListAsync();
            return items;
        }
        public string GetItemName(int itemid)
        {
            var item = _context.Items.FirstOrDefault(b => b.Id == itemid);
            return item.Name;
        }
        public string GetItemRarity(int itemid)
        {
            var item = _context.Items.FirstOrDefault(b => b.Id == itemid);
            return item.Rarity.ToString();
        }
        public float GetItemPrice(int itemid)
        {
            var item = _context.Items.FirstOrDefault(b => b.Id == itemid);
            return item.Price;
        }

    }
}
