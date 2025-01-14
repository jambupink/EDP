namespace LearningAPI.Models
{
	public class Payment
	{
		public int PaymentId { get; set; }
		public int OrderId { get; set; }
		public string PaymentMethod { get; set; }
		public string CustomerName { get; set; }
		public string Cvc { get; set; }
		public decimal Amount { get; set; }
		public DateTime PaymentDate { get; set; }
		public string Status { get; set; } = "Pending";
		public Order Order { get; set; }
	}

}
