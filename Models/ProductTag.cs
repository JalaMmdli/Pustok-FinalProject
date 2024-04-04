using System;
using Pustok.Models.baseModels;

namespace Pustok.Models
{
	public class ProductTag:BaseModel
	{
		public int ProductId { get; set; }
		public Product Product { get; set; } = null!;
		public int TagId { get; set; }
		public Tag Tag { get; set; } = null!;
    }
}

