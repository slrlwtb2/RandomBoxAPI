using LootBoxAPI.Data;
using LootBoxAPI.Models;
using LootBoxAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LootBoxAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context= context;
        }


        public User CreateUser(string username, byte[] passwordHash, byte[] passwordSalt)
        {
            User user = new User()
            {
                Username = username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            return user;
        }

        public async Task<User> GetUserbyId(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<List<object>> GetUsers()
        {
            return _context.Users
                   .OrderBy(u => u.Id)
                   .Select(u => new
                    {
                     u.Id,
                     u.Username,
                     u.Balance,
                     u.Role
                    })
                   .ToListAsync<object>();
        }

        public bool hasUsername(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }


        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task<float> GetBalance(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user.Balance;
        }

        Task<User> IUserRepository.GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> UserExist(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user != null;
        }
        public string GetUsername(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            return user.Username;
        }
    }
    
}
