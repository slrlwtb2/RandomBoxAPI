using DocumentFormat.OpenXml.Bibliography;
using LootBoxAPI.Data;
using LootBoxAPI.Models;
using LootBoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RandomBoxAPI.DTO;
using RandomBoxAPI.Filters;
using RandomBoxAPI.Repository.Interfaces;
using System.Security.Claims;

namespace LootBoxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IRepository<Inventory,int> _inventoryRepository;
        private readonly IInventoryService _inventoryService;
        private readonly IItemService _itemService;
        private readonly IRepository<Item,int> _itemsRepository;
        private readonly IRepository<User, int> _userRepository;
        private readonly ApplicationDbContext _context;

        public InventoryController(
            IRepository<Inventory, int> inventoryRepository,
            IInventoryService inventoryService,
            IRepository<Item, int> itemsRepository,
            IItemService itemService,
            ApplicationDbContext context,
            IRepository<User, int> userRepository)
        {
            _inventoryRepository = inventoryRepository;
            _inventoryService = inventoryService;
            _context = context;
            _itemService = itemService;
            _itemsRepository = itemsRepository;
            _userRepository = userRepository;
        }

        // GET: api/Inventory/GetById
        [HttpGet("GetById"), Authorize]
        [User_ValidateUserIdFilter]
        public async Task<IActionResult> GetbyId()
        {
            try
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
                var inv = await _inventoryService.GetInventoryItemListByUserIdAsync(userId);
                return Ok(inv);
            }
            catch (Exception)
            {

                return StatusCode(500, "An unexpected error occurred while deleting the product.Please try again later.");
            }
        }

        // POST: api/Inventory/AddItemtoInv
        [HttpPost("AddItemtoInv/{itemId}/{amount}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddItemtoInventory(int itemId, int userId, int amount)
        {
            try
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
            catch (Exception)
            {

                return StatusCode(500, "An unexpected error occurred while deleting the product.Please try again later.");
            }
        }

        // PUT: api/Inventory/SellItem
        [HttpPut("SellItemfromInventory/{itemId}/amount"), Authorize]
        public async Task<IActionResult> SellItemfromInventory(int itemId, int amount)
        {
            try
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
            catch (Exception)
            {

                return StatusCode(500, "An unexpected error occurred while deleting the product.Please try again later.");
            }
        }

        // PUT: api/Item/OpenBox
        [HttpPut("openbox/{boxid}"), Authorize]
        public async Task<IActionResult> OpenBox(int boxid)
        {
            try
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
                    var box = await _itemsRepository.GetById(boxid);

                    if (_itemService.AreBox(box))
                    {
                        var listOfItems = await _itemService.GetItemInBox(boxid);
                        var randomItem = await _inventoryService.OpenBox(listOfItems);
                        ItemDTO itemDTO = new ItemDTO()
                        {
                            Id = randomItem.Id,
                            Name = randomItem.Name,
                            Rarity = randomItem.Rarity.ToString(),
                            Price = randomItem.Price,
                            ImageData = randomItem.ImageData.IsNullOrEmpty() ? false : true

                        };
                        if (await _inventoryService.AlreadyHaveItem(randomItem.Id, userId))
                        {
                            var updateinventory = await _context.Inventories
                            .FirstOrDefaultAsync(inv => inv.UserId == userId && inv.ItemId == randomItem.Id);

                            updateinventory.Quantity += 1;
                            _context.SaveChanges();
                        }
                        else
                        {
                            Inventory Inventory = new Inventory()
                            {
                                UserId = userId,
                                ItemId = randomItem.Id,
                                Quantity = 1
                            };
                            await _inventoryRepository.Add(Inventory);
                            _context.SaveChanges();
                        }
                        var usedBoxItem = await _context.Inventories
                        .FirstOrDefaultAsync(inv => inv.UserId == userId && inv.ItemId == boxid);

                        usedBoxItem.Quantity -= 1;
                        _context.SaveChanges();
                        return Ok(itemDTO);
                    }
                }

                return NotFound($"Controller: not found itemid {boxid}");
            }
            catch (Exception)
            {

                return StatusCode(500, "An unexpected error occurred while deleting the product.Please try again later.");
            }
        }
        // PUT: api/Item/BuyBox
        [HttpPut("BuyBox/{boxId}")]
        [Authorize]
        public async Task<IActionResult> BuyBox(int boxId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return BadRequest("Invalid token");
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    return BadRequest("Invalid userId in token");
                }

                if (!await _userRepository.Exist(userId))
                {
                    return NotFound($"Controller: UserId {userId} does not exist");
                }

                Item box = await _itemsRepository.GetById(boxId);
                if (!_itemService.AreBox(box))
                {
                    return BadRequest($"Controller: boxId {boxId} is not a box");
                }

                User user = await _userRepository.GetById(userId);
                if (user.Balance < box.Price)
                {
                    return BadRequest($"Controller: Insufficient balance, {user.Username} doesn't have enough credits");
                }

                user.Balance -= box.Price;

                var updateinventory = await _context.Inventories
                    .FirstOrDefaultAsync(inv => inv.UserId == userId && inv.ItemId == boxId);

                if (updateinventory != null)
                {
                    updateinventory.Quantity += 1;
                }
                else
                {
                    var inventory = new Inventory
                    {
                        UserId = userId,
                        ItemId = boxId,
                        Quantity = 1
                    };
                    await _inventoryRepository.Add(inventory);
                }
                _context.SaveChanges();

                return Ok(updateinventory);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred while processing the request. Please try again later.");
            }
        }

    }
}

