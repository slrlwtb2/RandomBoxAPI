﻿using LootBoxAPI.Data;
using LootBoxAPI.Models;
using LootBoxAPI.Repository.Interfaces;
using LootBoxAPI.Services;
using LootBoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RandomBoxAPI.DTO;
using RandomBoxAPI.Models;
using System.Threading.Tasks;

namespace LootBoxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemRepository _itemsRepository;
        private readonly IItemService _itemsService;
        private readonly ApplicationDbContext _context;

        public ItemController(
            IItemRepository itemListRepository,
            IItemService itemListService,
            ApplicationDbContext context)
        {
            _itemsRepository = itemListRepository;
            _itemsService = itemListService;
            _context = context;
        }

        // GET: api/Item/GetItems
        [HttpGet("GetItems"), Authorize]
        public async Task<IActionResult> GetItems()
        {
            var items = await _itemsRepository.GetItemsAsync();
            return Ok(items);
        }
        [HttpGet("GetItemNameById/{itemId}"), Authorize]
        public async Task<IActionResult> GetItemNameById(int itemId)
        {
            var item = await _itemsRepository.GetItembyIdAsync(itemId);
            return Ok(item.Name);
        }
        // GET: api/Item/GetItemDiscriminator
        [HttpGet("GetItemDiscriminator/{itemId}")]
        public async Task<IActionResult> GetItemDiscriminator(int itemId)
        {
            if (await _itemsRepository.ItemExist(itemId))
            {
                var item = await _itemsRepository.GetItembyIdAsync(itemId);
                if (_itemsService.AreBox(item))
                {
                    return Ok("Box");
                }
                else
                {
                    return Ok("Item");
                }
            }
            else
            {
                return NotFound($"Controller: itemId: {itemId} Not Found");
            }
        }

        // POST: api/Item/CreateItem
        [HttpPost("CreateItem/{name}/{rarity}/{price}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateItem(string name, int rarity, float price)
        {
            var item = _itemsService.CreateItem(name, rarity, price);
            await _itemsRepository.InsertItem(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        // POST: api/Item/CreateBox
        [HttpPost("CreateBox/{name}/{rarity}/{price}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBox(string name, int rarity, float price)
        {
            var item = _itemsService.CreateBox(name, rarity, price);
            await _itemsRepository.InsertItem(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        // DELETE: api/Item/DeleteItem/{id}
        [HttpDelete("DeleteItem/{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _itemsRepository.GetItembyIdAsync(id);
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(item.Name + " deleted");
        }

        // PUT: api/Item/UpdateItem
        [HttpPut("UpdateItem/{id}/{name}/{rarity}/{price}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateItem(int id, string name, int rarity, float price)
        {
            if (await _itemsRepository.ItemExist(id))
            {
                var original = await _itemsRepository.GetItembyIdAsync(id);
                Item update = _itemsService.CreateItem(name, rarity, price);
                update.Id = id;
                await _itemsRepository.UpdateItem(original, update);
                await _context.SaveChangesAsync();
                return Ok("Item updated");
            }
            else
            {
                return NotFound($"No item with id {id}");
            }
        }

        // POST: api/Item/AddItemToBox
        [HttpPost("AddItemToBox/{boxId}/{itemId}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddItemToBox(int boxId, int itemId)
        {
            Item box;
            Item item;
            BoxItem boxItem = new BoxItem();
            if (await _itemsService.BoxAndItemExist(boxId, itemId))
            {
                box = await _itemsRepository.GetItembyIdAsync(boxId);
                item = await _itemsRepository.GetItembyIdAsync(itemId);
            }
            else
            {
                return NotFound($"BoxId {boxId} or ItemId {itemId} is not found");
            }

            if (_itemsService.AreBox(box) && _itemsService.AreItem(item))
            {
                if (!await _itemsService.AlreadyContainItemInBox(boxId, itemId))
                {
                    boxItem = _itemsService.CreateBoxItemList(boxItem, boxId, itemId);
                    _context.BoxItems.Add(boxItem);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return BadRequest($"Item '{item.Name}' already exists in box '{box.Name}' (Id: {box.Id})");
                }
            }
            else
            {
                return BadRequest("Type of Box or Item is not matching");
            }

            return Ok(boxItem);
        }

        // GET: api/Item/GetBoxes
        [HttpGet("GetBoxes"), Authorize]
        public async Task<IActionResult> GetBoxes()
        {
            //var boxes = await _context.Items.Where(i => i.Discriminator == "Box").ToListAsync();
            var boxes = await _context.Items.Where(i => i.Discriminator == "Box").Select(i => new ItemDTO()
            {
                Id = i.Id,
                Name = i.Name,
                Rarity = i.Rarity.ToString(),
                Price = i.Price,
                ImageData = i.ImageData.IsNullOrEmpty() ? false:true
            }).ToListAsync();
            return Ok(boxes);
        }

        // GET: api/Item/GetItemsinBox
        [HttpGet("GetItemsinBox/{boxId}"), Authorize]
        public async Task<IActionResult> GetItemsInBox(int boxId)
        {
            var items = await _itemsRepository.GetItemInBox(boxId);
            return Ok(items);
        }

        // POST: api/Item/UploadPicture
        [HttpPost("UploadPicture/{itemid}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadPicture(IFormFile file,int itemid)
        {
            if (file != null && file.Length > 0)
            {
                if (await _itemsRepository.ItemExist(itemid))
                {
                    var item = await _itemsRepository.GetItembyIdAsync(itemid);
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        item.ImageData = memoryStream.ToArray();
                        await _context.SaveChangesAsync();

                        return Ok();
                    } 
                }
                else
                {
                    return BadRequest($"Controller:No itemid  {itemid} exist");
                }
            }

            return BadRequest("No picture provided.");
        }
        // GET: api/Item/GetImage/{id}
        [HttpGet("GetImage/{itemid}")]
        public async Task<IActionResult> GetImage(int itemid)
        {
            
            if (await _itemsRepository.ItemExist(itemid))
            {
                var item = await _context.Items.FirstOrDefaultAsync(p => p.Id == itemid);
                return File(item.ImageData, "image/png");
            }
            else
            {
                return NotFound($"Controller:No itemid  {itemid} exist");
            }
        }

    }
}
