using System;
using Pustok.Models;

namespace Pustok.ViewModels;

public class HomeVm
{
	public List<Product> Products { get; set; }
	public List<Category> Categories { get; set; }
	public List<Slider> Sliders { get; set; }
	public List<Service> Services { get; set; }
}

