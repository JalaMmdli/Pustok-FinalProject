using System;
using Pustok.Models.baseModels;

namespace Pustok.Models;

public class Brand : BaseModel
{
    public string Name { get; set; } = null!;
    public ICollection<Product> Products { get; set; }
    public Brand()
    {
        Products = new HashSet<Product>();
    }

}

