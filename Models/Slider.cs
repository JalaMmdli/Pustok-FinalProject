using System;
using System.ComponentModel.DataAnnotations.Schema;
using Pustok.Models.baseModels;

namespace Pustok.Models
{
	public class Slider:BaseModel
	{
		public string Title { get; set; } = null!;
		public string Description { get; set; } = null!;
        public string Button { get; set; } = null!;
        public string ImagePath { get; set; } = null!;

	}
}

