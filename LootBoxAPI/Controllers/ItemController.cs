using LootBoxAPI.Data;
using LootBoxAPI.Models;
using LootBoxAPI.Services;
using LootBoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RandomBoxAPI.DTO;
using RandomBoxAPI.Models;
using RandomBoxAPI.Repository.Interfaces;
using System.Threading.Tasks;

namespace LootBoxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IRepository<Item,int> _itemsRepository;
        private readonly IItemService _itemsService;
        private readonly ApplicationDbContext _context;

        public ItemController(
            IRepository<Item, int> itemListRepository,
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
            try
            {
                var items = await _itemsRepository.GetAll();
                return Ok(items);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet("GetItemNameById/{itemId}"), Authorize]
        public async Task<IActionResult> GetItemNameById(int itemId)
        {
            try
            {
                var item = await _itemsRepository.GetById(itemId);
                return Ok(item.Name);
            }
            catch (Exception)
            {

                throw;
            }
        }
        // GET: api/Item/GetItemDiscriminator
        [HttpGet("GetItemDiscriminator/{itemId}")]
        public async Task<IActionResult> GetItemDiscriminator(int itemId)
        {
            try
            {
                if (await _itemsRepository.Exist(itemId))
                {
                    var item = await _itemsRepository.GetById(itemId);
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
            catch (Exception)
            {

                throw;
            }
        }

        // POST: api/Item/CreateItem
        [HttpPost("CreateItem/{name}/{rarity}/{price}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateItem(string name, int rarity, float price)
        {
            try
            {
                var item = _itemsService.CreateItem(name, rarity, price);
                await _itemsRepository.Add(item);
                await _context.SaveChangesAsync();
                return Ok(item);
            }
            catch (Exception)
            {

                throw;
            }
        }

        // POST: api/Item/CreateBox
        [HttpPost("CreateBox/{name}/{rarity}/{price}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBox(string name, int rarity, float price)
        {
            try
            {
                var item = _itemsService.CreateBox(name, rarity, price);
                await _itemsRepository.Add(item);
                await _context.SaveChangesAsync();
                return Ok(item);
            }
            catch (Exception)
            {

                throw;
            }
        }

        // DELETE: api/Item/DeleteItem/{id}
        [HttpDelete("DeleteItem/{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var item = await _itemsRepository.GetById(id);
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
                return Ok(item.Name + " deleted");
            }
            catch (Exception)
            {

                throw;
            }
        }

        // PUT: api/Item/UpdateItem
        [HttpPut("UpdateItem/{id}/{name}/{rarity}/{price}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateItem(int id, string name, int rarity, float price)
        {
            try
            {
                var existingItem = await _itemsRepository.GetById(id);

                if (existingItem != null)
                {
                    existingItem.Name = name;
                    existingItem.Rarity = (Item.Tier)rarity;
                    existingItem.Price = price;

                    _itemsRepository.Update(existingItem);
                    await _context.SaveChangesAsync();

                    return Ok("Item updated");
                }
                else
                {
                    return NotFound($"No item with id {id}");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        // POST: api/Item/AddItemToBox
        [HttpPost("AddItemToBox/{boxId}/{itemId}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddItemToBox(int boxId, int itemId)
        {
            try
            {
                Item box;
                Item item;
                BoxItem boxItem = new BoxItem();
                if (await _itemsService.BoxAndItemExist(boxId, itemId))
                {
                    box = await _itemsRepository.GetById(boxId);
                    item = await _itemsRepository.GetById(itemId);
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
            catch (Exception)
            {

                throw;
            }
        }

        // GET: api/Item/GetBoxes
        [HttpGet("GetBoxes"), Authorize]
        public async Task<IActionResult> GetBoxes()
        {
            try
            {
                var boxes = await _context.Items.Where(i => i.Discriminator == "Box").Select(i => new ItemDTO()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Rarity = i.Rarity.ToString(),
                    Price = i.Price,
                    ImageData = i.ImageData.IsNullOrEmpty() ? false : true
                }).ToListAsync();
                return Ok(boxes);
            }
            catch (Exception)
            {

                throw;
            }
        }

        // GET: api/Item/GetItemsinBox
        [HttpGet("GetItemsinBox/{boxId}"), Authorize]
        public async Task<IActionResult> GetItemsInBox(int boxId)
        {
            try
            {
                var items = await _itemsService.GetItemInBox(boxId);
                return Ok(items);
            }
            catch (Exception)
            {

                throw;
            }
        }

        // POST: api/Item/UploadPicture
        [HttpPost("UploadPicture/{itemid}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadPicture(IFormFile file,int itemid)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    if (await _itemsRepository.Exist(itemid))
                    {
                        var item = await _itemsRepository.GetById(itemid);
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
            catch (Exception)
            {

                throw;
            }
        }
        // GET: api/Item/GetImage/{id}
        [HttpGet("GetImage/{itemid}")]
        public async Task<IActionResult> GetImage(int itemid)
        {

            try
            {
                if (await _itemsRepository.Exist(itemid))
                {
                    var item = await _context.Items.FirstOrDefaultAsync(p => p.Id == itemid);
                    return File(item.ImageData, "image/png");
                }
                else
                {
                    return NotFound($"Controller:No itemid  {itemid} exist");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
