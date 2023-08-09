using LootBoxAPI.Data;
using LootBoxAPI.DTO;
using LootBoxAPI.Models;
using LootBoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RandomBoxAPI.Repository.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LootBoxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepository<User,int> _userRepository;
        private readonly IUserService _userService;
        private readonly IRepository<Inventory,int> _inventoryRepository;
        private readonly ApplicationDbContext _context;

        public UserController(IRepository<User,int> userRepository, IUserService userService, IRepository<Inventory,int> inventoryRepository, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _userService = userService;
            _inventoryRepository = inventoryRepository;
            _context = context;
        }

        // GET: api/User/getUsers
        [HttpGet("GetUsers"), Authorize(Roles =("Admin"))]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userRepository.GetAll();
                return Ok(users);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("GetBalance"), Authorize]
        public async Task<IActionResult> GetBalance()
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
            var balance = await _userService.GetBalance(userId);
            return Ok(balance);
        }

        // POST: api/User/register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO register_request)
        {
            try
            {
                if (_userService.hasUsername(register_request.Username))
                {
                    return BadRequest("User already exists");
                }

                _userService.CreatePasswordHash(register_request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                var user = _userService.CreateUser(register_request.Username, passwordHash, passwordSalt);
                await _context.AddAsync(user);
                _userRepository.Save();
                return Ok("User created");
            }
            catch (Exception)
            {
                throw;
            }
        }

        // POST: api/User/login
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(RegisterDTO login_request)
        {
            try
            {
                if (!_userService.hasUsername(login_request.Username))
                {
                    return BadRequest("Username or password is incorrect. Please try again.");
                }

                User user = await _userService.GetUserByUsername(login_request.Username);
                if (!_userService.VarifyPasswordHash(login_request.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return BadRequest("Username or password is incorrect. Please try again.");
                }

                string token = _userService.CreateToken(user);
                return Ok(token);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
