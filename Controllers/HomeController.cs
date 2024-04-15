using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Data;
using Pustok.ViewModels;

namespace Pustok.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        HomeVm vm = new();
       
        vm.Sliders = await _context.Sliders.ToListAsync();
        vm.Services = await _context.Services.ToListAsync();
        vm.NewProducts = await _context.Products.Include(x => x.Category).Include(x => x.ProductImgs).Include(x => x.Brand).Include(x=>x.Author).OrderByDescending(x => x.Id).Take(12).ToListAsync();
        vm.DiscountedProducts = await _context.Products.Include(x => x.Category).Include(x => x.ProductImgs).Include(x => x.Author).Include(x => x.Brand).OrderByDescending(x => x.Discount).Take(12).ToListAsync();
        vm.UndiscountedProducts = await _context.Products.Include(x => x.Category).Include(x => x.ProductImgs).Include(x => x.Author).Include(x => x.Brand).Where(x => x.Discount == 0).Take(12).ToListAsync();
        vm.ChildrenProducts = await _context.Products.Include(x => x.Category).Include(x => x.ProductImgs).Include(x => x.Author).Include(x => x.Brand).Where(x => x.Category.Name == "Children").Take(6).ToListAsync();
        vm.TwentyDiscountedProducts = await _context.Products.Include(x => x.Category).Include(x => x.ProductImgs).Include(x => x.Author).Include(x => x.Brand).Where(x => x.Discount == 20).Take(6).ToListAsync();
        return View(vm);
    }

}

