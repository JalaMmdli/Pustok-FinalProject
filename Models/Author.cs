using System;
using Pustok.Models.baseModels;

namespace Pustok.Models
{
	public class Author:BaseModel
	{
		public string Name { get; set; } = null!;
		public bool SoftDelete { get; set; }
		public IEnumerable<Product> Products { get; set; } = new List<Product>();
	}
}

