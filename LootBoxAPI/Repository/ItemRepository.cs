using LootBoxAPI.Data;
using LootBoxAPI.Models;
using LootBoxAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RandomBoxAPI.DTO;
using RandomBoxAPI.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LootBoxAPI.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _context;
        public ItemRepository(ApplicationDbContext context)
        {
            _context= context;
        } 
        public async Task InsertItem(Item item)
        {
            await _context.Items.AddAsync(item);
        }

        public void DeleteItemAsync(int id)
        {
            var item = _context.Items.FirstOrDefault(i => i.Id == id);
            _context.Items.Remove(item);
        }

        public async Task<Item> GetItembyIdAsync(int id)
        {
            return await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<ItemDTO>> GetItemsAsync()
        {
           var items = await _context.Items
          .Where(i => i.Discriminator == "Item")
          .Select(i => new ItemDTO
          {
              Id = i.Id,
              Name = i.Name,
              Rarity = i.Rarity.ToString(),
              Price = i.Price,
              ImageData = i.ImageData.IsNullOrEmpty() ? false : true
          }).ToListAsync();
            return items;
        }

        public Task<Item> GetItemsbyNameAsync(string name)
        {
            return _context.Items.FirstOrDefaultAsync(i => i.Name == name);
        }

        public void Save()
        {
            _context.SaveChanges();
            
        }

        public async Task UpdateItem(Item original,Item update)
        {
            var existingItem = await GetItembyIdAsync(original.Id);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(update);
                _context.Entry(existingItem).State = EntityState.Modified;
            }

        }

        public async Task<bool> ItemExist(int id)
        {
            var item = await _context.Items.FindAsync(id);
            return item != null;
        }
        public async Task<List<BoxItem>> GetItemInBox(int boxid)
        {
            var items = await _context.BoxItems.Where(b => b.BoxId == boxid).ToListAsync();
            return items;
        }
        public string GetItemName(int itemid)
        {
            var item =  _context.Items.FirstOrDefault(b => b.Id == itemid);
            return item.Name;
        }
    }
}
