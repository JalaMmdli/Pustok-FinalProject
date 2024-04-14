using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.Data;
using Pustok.Models;

namespace Pustok.Controllers;

public class BasketController : Controller
{
    private readonly AppDbContext _context;
    public BasketController(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {


        if (User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var basketItems = await _context.BasketItems.Include(x => x.Product).ThenInclude(x => x.ProductImgs).Where(x => x.AppUserId == userId).ToListAsync();
            return View(basketItems);

        }

        var basktItms = _getBasket();
        foreach (var item in basktItms)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);
            item.Product = product;

        }
        return View(basktItms);

    }

    public async Task<IActionResult> RemoveToBasket(int id,string? returnUrl)
    {
        if (User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var basketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.ProductId == id && x.AppUserId == userId);
            if (basketItem is null )
            {
                return NotFound();
            }

            _context.BasketItems.Remove(basketItem);
            await _context.SaveChangesAsync();

            if (returnUrl is not null)
                return Redirect(returnUrl);

            return RedirectToAction("index");
        }
        var basketItms = _getBasket();

        var basketItm = basketItms.FirstOrDefault(x => x.ProductId == id);
        if (basketItm is null)
            return NotFound();

        basketItms.Remove(basketItm);
        var json = JsonConvert.SerializeObject(basketItms);
        Response.Cookies.Append("basket", json);

        if (returnUrl is not null)
            return Redirect(returnUrl);

        return RedirectToAction("index");
    }

    private List<BasketItem> _getBasket()
    {
        List<BasketItem> basketItems = new();
        if (Request.Cookies["basket"] != null)
        {
            basketItems = JsonConvert.DeserializeObject<List<BasketItem>>(Request.Cookies["basket"]) ?? new();
        }

        return basketItems;
    }
}

