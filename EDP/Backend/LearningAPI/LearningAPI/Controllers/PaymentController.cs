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
	public class PaymentController : ControllerBase
	{
		private readonly MyDbContext _context;

		public PaymentController(MyDbContext context)
		{
			_context = context;
		}

		// POST: Make Payment
		[HttpPost]
		public async Task<ActionResult<Payment>> MakePayment(Payment payment)
		{
			if (payment == null || payment.OrderId <= 0 || payment.Amount <= 0 || string.IsNullOrEmpty(payment.PaymentMethod))
			{
				return BadRequest("Invalid payment details.");
			}

			payment.PaymentDate = DateTime.UtcNow;
			_context.Payments.Add(payment);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetPaymentDetails), new { paymentId = payment.PaymentId }, payment);
		}

		// GET: Get Payment Details
		[HttpGet("{paymentId}")]
		public async Task<ActionResult<Payment>> GetPaymentDetails(int paymentId)
		{
			var payment = await _context.Payments.FindAsync(paymentId);

			if (payment == null)
			{
				return NotFound("Payment not found.");
			}

			return Ok(payment);
		}
	}


}