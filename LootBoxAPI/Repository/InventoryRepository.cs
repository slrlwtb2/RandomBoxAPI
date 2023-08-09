using DocumentFormat.OpenXml.Office2010.Excel;
using LootBoxAPI.Data;
using LootBoxAPI.DTO;
using LootBoxAPI.Models;
using LootBoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using RandomBoxAPI.Repository.Interfaces;

namespace LootBoxAPI.Repository
{
    public class InventoryRepository : IRepository<Inventory, int>
    {
        private readonly ApplicationDbContext _context;
        public InventoryRepository (ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Inventory>> GetAll()
        {
            return  await _context.Inventories.ToListAsync();
        }

        public async Task<Inventory> GetById(int id)
        {
            return await _context.Inventories.FindAsync(id);
        }

        public async Task Delete(int id)
        {
            var inv = await _context.Inventories.FirstOrDefaultAsync(i => i.Id == id);
            if (inv != null) _context.Inventories.Remove(inv);
        }



        public void Update(Inventory entity)
        {
            _context.Inventories.Update(entity);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Add(Inventory entity)
        {
            await _context.Inventories.AddAsync(entity);
        }

        public async Task<bool> Exist(int id)
        {
            var inv = await _context.Inventories.FirstOrDefaultAsync(i => i.Id == id);
            if (inv != null) return true;
            return false;
        }
    }
}
