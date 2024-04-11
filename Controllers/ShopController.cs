using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Data;
using Pustok.ViewModels;
using Newtonsoft.Json;

namespace Pustok.Controllers;

public class ShopController : Controller
{
    private readonly AppDbContext _context;

    public ShopController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.Include(x => x.Category)
                                              .Include(x => x.Author)
                                              .Include(x => x.Brand)
                                              .Include(x => x.ProductImgs)
                                              .ToListAsync();
        return View(products);
    }

    public async Task<IActionResult> AddToBasket(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
            return NotFound();

        List<BasketVm> basketItems = GetBasket();

        var existItem = basketItems.FirstOrDefault(x => x.ProductId == id);

        if (existItem is not null)
            existItem.Count++;
        else
        {
            BasketVm vm = new() { ProductId = id, Count = 1 };
            basketItems.Add(vm);
        }

        var json = JsonConvert.SerializeObject(basketItems);
        Response.Cookies.Append("basket", json);


        return RedirectToAction(nameof(Index));

    }




    private List<BasketVm> GetBasket()
    {
        List<BasketVm> basketItems = new();
        if (Request.Cookies["basket"] != null)
        {
            basketItems = JsonConvert.DeserializeObject<List<BasketVm>>(Request.Cookies["basket"]) ?? new();
        }

        return basketItems;
    }
}

