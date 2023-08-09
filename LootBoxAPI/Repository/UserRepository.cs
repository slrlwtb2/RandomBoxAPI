using LootBoxAPI.Data;
using LootBoxAPI.Models;
using Microsoft.EntityFrameworkCore;
using RandomBoxAPI.Repository.Interfaces;

namespace LootBoxAPI.Repository
{
    public class UserRepository : IRepository<User,int>
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context= context;
        }

        public async Task Add(User entity)
        {
             await _context.Users.AddAsync(entity);
        }

        public async Task Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id); 
            _context.Users.Remove(user);
        }

        public async Task<bool> Exist(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != null) return true;
            return false;
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(User entity)
        {
            _context.Users.Update(entity);
        }
    }
    
}
