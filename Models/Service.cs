using System;
using Pustok.Models.baseModels;

namespace Pustok.Models
{
	public class Service:BaseModel
	{
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }
}

