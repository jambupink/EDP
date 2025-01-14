using AutoMapper;
using LearningAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LearningAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CartController : ControllerBase
	{
		private readonly MyDbContext _context;

		public CartController(MyDbContext context)
		{
			_context = context;
		}

		// POST: Add item to Cart
		[HttpPost]
		public async Task<ActionResult<Cart>> AddToCart(Cart cart)
		{
			if (cart == null || cart.UserId <= 0 || cart.ProductId <= 0 || cart.Quantity <= 0)
			{
				return BadRequest("Invalid cart details.");
			}

			_context.Carts.Add(cart);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetCartByUserId), new { userId = cart.UserId }, cart);
		}

		// GET: View Cart by User ID
		[HttpGet("{userId}")]
		public async Task<ActionResult<IEnumerable<Cart>>> GetCartByUserId(int userId)
		{
			var cart = await _context.Carts.Where(c => c.UserId == userId).ToListAsync();

			if (!cart.Any())
			{
				return NotFound("Cart is empty.");
			}

			return Ok(cart);
		}

		[HttpGet("{cartId}")]
		public async Task<ActionResult<Cart>> GetCartById(int cartId)
		{
			var cartItem = await _context.Carts.FindAsync(cartId);

			if (cartItem == null)
			{
				return NotFound("Cart item not found.");
			}

			return Ok(cartItem);
		}


		// PUT: Update Cart Item Quantity
		[HttpPut("{cartId}")]
		public async Task<IActionResult> UpdateCartItem(int cartId, [FromBody] int quantity)
		{
			var cartItem = await _context.Carts.FindAsync(cartId);

			if (cartItem == null)
			{
				return NotFound("Cart item not found.");
			}

			if (quantity <= 0)
			{
				return BadRequest("Quantity must be greater than 0.");
			}

			cartItem.Quantity = quantity;
			await _context.SaveChangesAsync();
			return NoContent();
		}

		// DELETE: Remove Cart Item
		[HttpDelete("{cartId}")]
		public async Task<IActionResult> RemoveCartItem(int cartId)
		{
			var cartItem = await _context.Carts.FindAsync(cartId);

			if (cartItem == null)
			{
				return NotFound("Cart item not found.");
			}

			_context.Carts.Remove(cartItem);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}


}