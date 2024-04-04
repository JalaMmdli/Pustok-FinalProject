﻿using System;
using Pustok.Models.baseModels;

namespace Pustok.Models
{
	public class Tag:BaseModel
	{
		public string Name { get; set; }
		public ICollection<Product> Products { get; set; }
	}
}

