using System;
using Pustok.Models.baseModels;

namespace Pustok.Models
{
	public class Service:BaseModel
	{
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Icon { get; set; } = null!;
    }
}

