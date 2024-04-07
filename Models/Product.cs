using System;
using Pustok.Models.baseModels;

namespace Pustok.Models;

public class Product : BaseModel
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
    public Brand Brand { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public int AuthorId  { get; set; }
    public Author Author { get; set; }
    public ICollection<ProductImg> ProductImgs { get; set; } = new List<ProductImg>();
    public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
    public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

}


