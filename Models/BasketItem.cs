using System;
using Pustok.Models.baseModels;

namespace Pustok.Models
{
	public class BasketItem:BaseModel
	{
		public int ProductId { get; set; }
		public Product Product { get; set; } = null!;
		public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
		public int Count { get; set; }
	}
}

