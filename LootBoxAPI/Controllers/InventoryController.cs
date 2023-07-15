﻿using LootBoxAPI.Data;
using LootBoxAPI.Models;
using LootBoxAPI.Repository.Interfaces;
using LootBoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RandomBoxAPI.DTO;
using System.Security.Claims;

namespace LootBoxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IInventoryService _inventoryService;
        private readonly IItemService _itemService;
        private readonly IItemRepository _itemsRepository;
        private readonly ApplicationDbContext _context;

        public InventoryController(
            IInventoryRepository inventoryRepository,
            IInventoryService inventoryService,
            IItemRepository itemsRepository,
            IItemService itemService,
            ApplicationDbContext context)
        {
            _inventoryRepository = inventoryRepository;
            _inventoryService = inventoryService;
            _context = context;
            _itemService = itemService;
            _itemsRepository = itemsRepository;
        }

        // GET: api/Inventory/GetById
        [HttpGet("GetById"), Authorize]
        public async Task<IActionResult> GetbyId()
        {
            // Retrieve the userId from the JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return BadRequest("Invalid token");
            }
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid userId in token");
            }
            var inv = await _inventoryRepository.GetInventoryItemListByUserIdAsync(userId);
            return Ok(inv);
        }

        // POST: api/Inventory/AddItemtoInv
        [HttpPost("AddItemtoInv/{itemId}/{amount}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddItemtoInventory(int itemId, int userId, int amount)
        {
            Inventory itemInv = new Inventory();
            if (await _inventoryService.AlreadyHaveItem(itemId, userId))
            {
                itemInv = await _inventoryService.UpdateItemtoInventory(itemId, userId, amount);
            }
            else
            {
                itemInv = await _inventoryService.AddItemtoInventory(itemId, userId, amount);
            }
            _context.SaveChanges();
            return Ok(itemInv);
        }

        // PUT: api/Inventory/SellItem
        [HttpPut("SellItemfromInventory/{itemId}/amount"), Authorize]
        public async Task<IActionResult> SellItemfromInventory(int itemId, int amount)
        {
            // Retrieve the userId from the JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return BadRequest("Invalid token");
            }
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid userId in token");
            }
            // SellItem Logic
            if (await _inventoryService.AlreadyHaveItem(itemId, userId))
            {
                await _inventoryService.SellItem(itemId, userId, amount);
            }
            else
            {
                return NotFound("Controller:Item not found in this inventory");
            }
            _context.SaveChanges();
            return Ok($"Item {itemId} have been sold");
        }

        // PUT: api/Item/OpenBox
        [HttpPut("openbox/{boxid}"), Authorize]
        public async Task<IActionResult> OpenBox(int boxid)
        {
            // Retrieve the userId from the JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return BadRequest("Invalid token");
            }
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid userId in token");
            }
            // OpenBox Logic
            if (await _inventoryService.AlreadyHaveItem(boxid, userId))
            {
                var box = await _itemsRepository.GetItembyIdAsync(boxid);

                if (_itemService.AreBox(box))
                {
                    var listOfItems = await _itemsRepository.GetItemInBox(boxid);
                    var item = await _inventoryService.OpenBox(listOfItems);
                    ItemDTO itemDTO = new ItemDTO()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Rarity = item.Rarity.ToString(),
                        Price = item.Price,
                        ImageData = item.ImageData.IsNullOrEmpty() ? false : true

                    };
                    if (await _inventoryService.AlreadyHaveItem(item.Id, userId))
                    {
                        var updateinventory = await _context.Inventories.FirstOrDefaultAsync(inv => inv.UserId == userId && inv.ItemId == item.Id);
                        updateinventory.Quantity += 1;
                        _context.SaveChanges();
                    }
                    else
                    {
                        Inventory Inventory = new Inventory()
                        {
                            UserId = userId,
                            ItemId = item.Id,
                            Quantity = 1
                        };
                        await _inventoryRepository.Add(Inventory);
                        _context.SaveChanges();
                    }
                    var inventory = await _context.Inventories.FirstOrDefaultAsync(inv => inv.UserId == userId && inv.ItemId == boxid);
                    inventory.Quantity -= 1;
                    _context.SaveChanges();
                    return Ok(itemDTO);
                }
            }

            return NotFound($"Controller: not found itemid {boxid}");
        }
        // PUT: api/Item/BuyBox
        [HttpPut("BuyBox/{boxId}"), Authorize]
        public async Task<IActionResult> BuyBox(int boxId)
        {
            // Retrieve the userId from the JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return BadRequest("Invalid token");
            }
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid userId in token");
            }
            // BuyBox Logic
            if (await _context.Users.AnyAsync(u => u.Id == userId))
            {
                Item box = await _itemsRepository.GetItembyIdAsync(boxId);
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (_itemService.AreBox(box))
                {
                    if (user.Balance>=box.Price)
                    {
                        user.Balance -= box.Price;
                        if (await _inventoryService.AlreadyHaveItem(boxId, userId))
                        {
                            var updateinventory = await _context.Inventories.FirstOrDefaultAsync(inv => inv.UserId == userId && inv.ItemId == boxId);
                            updateinventory.Quantity += 1;
                            _context.SaveChanges();
                            return Ok(updateinventory);
                        }
                        else
                        {
                            Inventory Inventory = new Inventory()
                            {
                                UserId = userId,
                                ItemId = boxId,
                                Quantity = 1
                            };
                            await _inventoryRepository.Add(Inventory);
                            _context.SaveChanges();
                            return Ok(Inventory);
                        } 
                    }
                    else
                    {
                       return BadRequest($"Controller: Insufficient balance, {user.Username} don't have enough credits");
                    }
                }
                else
                {
                    return BadRequest($"Controller: boxId {boxId} is not a box");
                }
            }
            else
            {
                return NotFound($"Controller:UserId {userId} does not exists");
            }
        }
    }
}