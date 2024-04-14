using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Pustok.Data;
using Pustok.Models;

namespace Pustok.Services;

public class LayoutService
{
    private readonly IHttpContextAccessor _httpContext;
    private readonly AppDbContext _context;

    private readonly UserManager<AppUser> _userManager;

    public LayoutService(IHttpContextAccessor httpContext, AppDbContext context, UserManager<AppUser> userManager)
    {
        _httpContext = httpContext;
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<BasketItem>> GetBasket()
    {
        if (_httpContext.HttpContext.User.Identity.IsAuthenticated)
        {
            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return new();
            }
            var basketItems = await _context.BasketItems.Include(x=>x.Product).ThenInclude(x=>x.ProductImgs).Where(x => x.AppUserId == userId).ToListAsync();
            return basketItems;

        }

        var basktItms = _getBasket();
        foreach (var item in basktItms)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);
            item.Product = product;

        }
        return basktItms;


    }
    public async Task<Dictionary<string,string>> GetSettingsAsync()
    {
        var settings = await _context.Settings.ToDictionaryAsync(x => x.Key, x => x.Value);

        return settings;
    }

    public async Task<List<Category>> GetCategories()
    {
        var categories = await _context.Categories.Where(x=>x.ParentId==null).Include(x=>x.Children).ToListAsync();

        return categories;
    }

    private List<BasketItem> _getBasket()
    {
        List<BasketItem> basketItems = new();
        if (_httpContext.HttpContext.Request.Cookies["basket"] != null)
        {
            basketItems = JsonConvert.DeserializeObject<List<BasketItem>>(_httpContext.HttpContext.Request.Cookies["basket"]) ?? new();
        }

        return basketItems;
    }
}

