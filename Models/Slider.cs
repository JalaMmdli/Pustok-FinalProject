using System;
using Pustok.Models.baseModels;

namespace Pustok.Models
{
	public class Slider:BaseModel
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string Button { get; set; }
		public string ImagePath { get; set; }
	}
}

