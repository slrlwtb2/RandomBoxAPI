﻿using LootBoxAPI.DTO;
using LootBoxAPI.Models;
using RandomBoxAPI.Models;
using static LootBoxAPI.Models.Item;

namespace LootBoxAPI.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<Inventory> AddItemtoInventory(int itemId, int userid,int amount);
        Task<Inventory> UpdateItemtoInventory(int itemId, int userId, int amount);
        Task<Inventory> SellItem(int itemId, int userId, int amount);
        Task<Inventory> DiscardItem(int itemId, int userId, int amount);
        Task<bool> AlreadyHaveItem(int itemId, int userId);
        Task<int> GetQuantity(int itemId, int userId);
        Task<Item> OpenBox(List<BoxItem> items);
        Task<bool> CheckUserAndItemExist(int userId, int itemId);
        Tier GetRandomRarity();
        Task RemoveItemInInventroy(int itemId, int userId);
        Task<List<InventoryDTO>> GetInventoryItemListByUserIdAsync(int id);
        Task<Inventory> AddOrUpdate(Inventory inventory);
    }
}
