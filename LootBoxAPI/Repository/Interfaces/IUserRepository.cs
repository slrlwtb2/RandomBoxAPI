using LootBoxAPI.Models;

namespace LootBoxAPI.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<List<object>> GetUsers();
        bool hasUsername(string username);
        User CreateUser(string username,byte[] passwordHash,byte[] passwordSalt);
        void Save();
        Task<float> GetBalance(int userId);
        Task<User> GetUserbyId(int id);
        Task<User> GetUserByUsername(string username);
        Task<bool> UserExist(int id);
        string GetUsername(int id);


    }
}
