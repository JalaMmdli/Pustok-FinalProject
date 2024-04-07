using System;
namespace Pustok.Areas.Admin.Dtos.ProductDtos
{
	public class ProductCreateDto
	{
        public string Name { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public int RevardPoints { get; set; }
        public bool IsStock { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Rating { get; set; }
        public string ShortDescription { get; set; } = null!;
        public string LongDescription { get; set; } = null!;
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public IFormFile MainFile { get; set; } = null!;
        public IFormFile HoverFile { get; set; } = null!;
        public List<IFormFile> AdditionalFiles { get; set; } = new();
        public List<int> TagIds { get; set; } = new();
    }
}

