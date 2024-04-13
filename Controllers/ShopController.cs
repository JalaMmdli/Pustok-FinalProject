using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Data;
using Pustok.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Pustok.Models;

namespace Pustok.Controllers;

public class ShopController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    public ShopController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
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


        if (User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return BadRequest();

            var dbBasketItems = await _context.BasketItems.Where(x => x.AppUserId == userId).ToListAsync();


            var existBItem = dbBasketItems.FirstOrDefault(x => x.ProductId == id);
            if (existBItem is not null)
            {
                existBItem.Count++;
                _context.BasketItems.Update(existBItem);
            }
            else
            {
                BasketItem bItem = new() { AppUserId = userId, ProductId = id, Count = 1 };
                await _context.BasketItems.AddAsync(bItem);
            }

            await _context.SaveChangesAsync();
        }
        else
        {


            List<BasketItem> basketItems = GetBasket();

            var existItem = basketItems.FirstOrDefault(x => x.ProductId == id);

            if (existItem is not null)
                existItem.Count++;
            else
            {
                BasketItem vm = new() { ProductId = id, Count = 1 };
                basketItems.Add(vm);
            }

            var json = JsonConvert.SerializeObject(basketItems);
            Response.Cookies.Append("basket", json);

        }

        return RedirectToAction(nameof(Index));

    }

    public async Task<IActionResult> Detail(int id)
    {

        var existProduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (existProduct is null) return BadRequest();


        var product = await _context.Products.Include(x => x.Category)
                                             .Include(x => x.ProductImgs)
                                             .Include(x => x.Brand)
                                             .Include(x => x.Author)
                                             .Include(x => x.ProductTags)
                                             .ThenInclude(x => x.Tag)
                                             .FirstOrDefaultAsync(x => x.Id == id);
        if (product is null) return NotFound();
        return View(product);
    }

    public async Task<IActionResult> Search(string search)
    {
        var products = await _context.Products.Where(x => x.Name.Trim().ToLower().Contains(search.ToLower().Trim()))
                                              .Include(x => x.Category)
                                              .Include(x => x.Author)
                                              .Include(x => x.Brand)
                                              .Include(x => x.ProductImgs)
                                              .ToListAsync();

        return View("Index",products);
    }

    private List<BasketItem> GetBasket()
    {
        List<BasketItem> basketItems = new();
        if (Request.Cookies["basket"] != null)
        {
            basketItems = JsonConvert.DeserializeObject<List<BasketItem>>(Request.Cookies["basket"]) ?? new();
        }

        return basketItems;
    }
}

