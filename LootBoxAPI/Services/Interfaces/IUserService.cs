using LootBoxAPI.Models;

namespace LootBoxAPI.Services.Interfaces
{
    public interface IUserService
    {
       void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
       bool VarifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
       string CreateToken(User user);
       void AddBalance(int userId,float amount);
       void RemoveBalance(int userId, float amount);
       User CreateUser(string username, byte[] passwordHash, byte[] passwordSalt);
       bool hasUsername(string username);
       Task<float> GetBalance(int userId);
       Task<string> GetUsername(int id);
        Task<User> GetUserByUsername(string username);

    }
}
