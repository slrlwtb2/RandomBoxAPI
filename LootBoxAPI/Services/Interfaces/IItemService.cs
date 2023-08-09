using LootBoxAPI.Models;
using RandomBoxAPI.Models;
using System.Threading.Tasks;
using static LootBoxAPI.Models.Item;

namespace LootBoxAPI.Services.Interfaces
{
    public interface IItemService
    {
        Item CreateItem(string name, int rarity, float price);
        Item CreateBox(string name, int rarity, float price);
        void CreateItemValidation(string name, int rarity, float price);
        Task<bool> BoxAndItemExist(int itemid1, int itemid2);
        bool AreBox(Item box);
        bool AreItem(Item item);
        Task<bool> AlreadyContainItemInBox(int boxid, int itemid);
        BoxItem CreateBoxItemList(BoxItem boxItem, int boxid, int itemid);
        public Task<List<BoxItem>> GetItemInBox(int boxid);
        float GetItemPrice(int itemid);
        string GetItemRarity(int itemid);
        public string GetItemName(int itemid);
    }
}
