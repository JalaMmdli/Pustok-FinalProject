using System;
using System.ComponentModel.DataAnnotations.Schema;
using Pustok.Models.baseModels;

namespace Pustok.Models
{
	public class ProductImg:BaseModel
	{
		public string Url { get; set; } = null!;
        [NotMapped]
        public IFormFile File { get; set; } = null!;
        public bool IsMain { get; set; }
		public bool IsHover { get; set; }
		public int ProductId { get; set; }
		public Product Product { get; set; } = null!;

    }
}

