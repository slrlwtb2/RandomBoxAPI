using LootBoxAPI.Data;
using LootBoxAPI.Models;
using LootBoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RandomBoxAPI.Repository.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LootBoxAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<User,int> _userRepository;
        private readonly ApplicationDbContext _context;
        public UserService(IConfiguration configuration,IRepository<User,int> userRepository,ApplicationDbContext context)
        {
            _configuration= configuration;
            _userRepository= userRepository;
            _context= context;
        }
        public async void AddBalance(int userId, float amount)
        {
           var user = await _userRepository.GetById(userId);
           if (user == null) { throw new ArgumentException("thers's no userID");}
           user.Balance += amount;

        }
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                passwordSalt = hmac.Key;
            }
        }
        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username),      
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        public void RemoveBalance(int userId, float amount)
        {
            throw new NotImplementedException();
        }
        public bool VarifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
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
        public bool hasUsername(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }
        public async Task<float> GetBalance(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user.Balance;
        }
        public async Task<string> GetUsername(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            return user.Username;
        }
        public async Task<User> GetUserByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
