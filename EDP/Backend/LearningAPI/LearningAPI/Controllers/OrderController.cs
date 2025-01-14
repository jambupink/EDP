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
	public class OrderController : ControllerBase
	{
		private readonly MyDbContext _context;

		public OrderController(MyDbContext context)
		{
			_context = context;
		}

		// POST: Place Order
		[HttpPost]
		public async Task<ActionResult<Order>> PlaceOrder(Order order)
		{
			if (order == null || order.UserId <= 0 || order.TotalPrice <= 0)
			{
				return BadRequest("Invalid order details.");
			}

			order.OrderStatus = "Pending";
			order.OrderDate = DateTime.UtcNow;
			_context.Orders.Add(order);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetOrdersByUserId), new { userId = order.UserId }, order);
		}

		// GET: Get Orders by User ID
		[HttpGet("{userId}")]
		public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUserId(int userId)
		{
			var orders = await _context.Orders.Where(o => o.UserId == userId).ToListAsync();

			if (!orders.Any())
			{
				return NotFound("No orders found.");
			}

			return Ok(orders);
		}

		[HttpGet("{orderId}")]
		public async Task<ActionResult<Order>> GetOrderById(int orderId)
		{
			var order = await _context.Orders.FindAsync(orderId);

			if (order == null)
			{
				return NotFound("Order not found.");
			}

			return Ok(order);
		}


		// DELETE: Cancel Order
		[HttpDelete("{orderId}")]
		public async Task<IActionResult> CancelOrder(int orderId)
		{
			var order = await _context.Orders.FindAsync(orderId);

			if (order == null)
			{
				return NotFound("Order not found.");
			}

			if (order.OrderStatus == "Shipped")
			{
				return BadRequest("Cannot cancel an order that has been shipped.");
			}

			_context.Orders.Remove(order);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}


}