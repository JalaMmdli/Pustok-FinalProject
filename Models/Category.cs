using System;
using Pustok.Models.baseModels;

namespace Pustok.Models
{
	public class Category:BaseModel
	{
		public string Name { get; set; } = null!;
        public int? ParentId { get; set; }
		public Category? Parent { get; set; }
		
		public ICollection<Product> Products { get; set; } = new List<Product>();
		public ICollection<Category> Children { get; set; } = new List<Category>();
		
	}
}

