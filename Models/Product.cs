using System;
using Pustok.Models.baseModels;

namespace Pustok.Models;

public class Product:BaseModel
{

	public string Name { get; set; } = null!;
	public string ProductCode { get; set; } = null!;
    public int RevardPoints { get; set; }
	public bool Isstock { get; set; }
	public decimal Price { get; set; }
	public decimal Discount { get; set; }
	public decimal Rating { get; set; }
	public string ShortDescription { get; set; } = null!;
	public string LongDescription { get; set; } = null!;
	public int BrandId { get; set; }
	public Brand Brand { get; set; }
	public int CategoryId { get; set; }
	public Category Category { get; set; }
	public ICollection<ProductImg> ProductImgs { get; set; } 
	public ICollection<Tag> Tags { get; set; }
	public Product()
	{
		ProductImgs = new(); 
            Tags= new HashSet<Tag>();


    }
}

