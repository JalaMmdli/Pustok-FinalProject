using System;
using Pustok.Models;

namespace Pustok.ViewModels;

public class HomeVm
{
	public List<Product> NewProducts { get; set; } = new();
	public List<Product> ChildrenProducts { get; set; } = new();
	public List<Product> DiscountedProducts { get; set; } = new();
	public List<Product> TwentyDiscountedProducts { get; set; } = new();
	public List<Product> UndiscountedProducts { get; set; } = new();
    public List<Slider> Sliders { get; set; } = new();
    public List<Service> Services { get; set; } = new();
}

