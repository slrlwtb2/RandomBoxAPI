using LootBoxAPI.Data;
using LootBoxAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RandomBoxAPI.DTO;
using RandomBoxAPI.Models;
using RandomBoxAPI.Repository.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LootBoxAPI.Repository
{
    public class ItemRepository : IRepository<Item,int>
    {
        private readonly ApplicationDbContext _context;
        public ItemRepository(ApplicationDbContext context)
        {
            _context= context;
        }
        public async Task Add(Item entity)
        {
            await _context.Items.AddAsync(entity);
        }

        public async Task Delete(int id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null) _context.Items.Remove(item);
        }

        public async Task<List<Item>> GetAll()
        {
            var items = await _context.Items.Select(i => new Item()
            {
                Id = i.Id,
                Name = i.Name,
                Rarity = i.Rarity,
                Discriminator= i.Discriminator,
                Price = i.Price,
                ImageData = i.ImageData.IsNullOrEmpty() ? new byte[] { 0 } : new byte[] { 1 }
            }).ToListAsync();
            return items;
        }

        public async Task<Item> GetById(int id)
        {
            var item = await _context.Items.FindAsync(id);
            return item;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Item entity)
        {
            _context.Items.Update(entity);
        }

        public async Task<bool> Exist(int id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
            if (item != null) return true;
            return false;
        }
    }
}
