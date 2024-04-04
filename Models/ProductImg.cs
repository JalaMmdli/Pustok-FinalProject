using System;
namespace Pustok.Models
{
	public class ProductImg
	{
		public string Url { get; set; } = null!;
		public bool UsMain { get; set; }
		public bool IsHover { get; set; }
		public int ProductId { get; set; }
		public Product Product { get; set; }

	}
}

