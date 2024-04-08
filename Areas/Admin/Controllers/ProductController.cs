using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Admin.Dtos;
using Pustok.Data;
using Pustok.Extensions;
using Pustok.Models;


namespace Pustok.Areas.Admin.Controllers;
[Area("Admin")]

public class ProductController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        List<Product> products = await _context.Products
                                               .Include(x => x.ProductImgs)
                                               .Include(x => x.Category)
                                               .Include(x => x.Brand)

                                               .ToListAsync();
        return View(products);


    }
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.Brands = await _context.Brands.ToListAsync();
        ViewBag.Author = await _context.Authors.ToListAsync();

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.Brands = await _context.Brands.ToListAsync();
        ViewBag.Author = await _context.Authors.ToListAsync();
        if (!ModelState.IsValid)
        {
            return View();
        }

        if (_context.Products.Any(x => x.Name == dto.Name))
        {
            ModelState.AddModelError("", "Product already exists");
            return View(dto);
        }


        foreach (var file in dto.AdditionalFiles)
        {

            if (!file.CheckFileSize(2))
            {
                ModelState.AddModelError("AdditionalFiles", "Files cannot be more than 2mb");
                return View(dto);
            }


            if (!file.CheckFileType("image"))
            {
                ModelState.AddModelError("AdditionalFiles", "Files must be image type!");
                return View(dto);
            }


        }

        if (!dto.MainFile.CheckFileSize(2))
        {
            ModelState.AddModelError("MainFile", "Files cannot be more than 2mb");
            return View(dto);
        }


        if (!dto.MainFile.CheckFileType("image"))
        {
            ModelState.AddModelError("MainFile", "Files must be image type!");
            return View(dto);
        }

        if (!dto.HoverFile.CheckFileSize(2))
        {
            ModelState.AddModelError("HoverFile", "Files cannot be more than 2mb");
            return View(dto);
        }


        if (!dto.HoverFile.CheckFileType("image"))
        {
            ModelState.AddModelError("HoverFile", "Files must be image type!");
            return View(dto);
        }


        Product product = new()
        {
            Name = dto.Name,
            ProductCode = dto.ProductCode,
            RevardPoints = dto.RevardPoints,
            IsStock = dto.IsStock,
            Price = dto.Price,
            Discount = dto.Discount,
            Rating = dto.Rating,
            ShortDescription = dto.ShortDescription,
            LongDescription = dto.LongDescription,
            BrandId = dto.BrandId,
            CategoryId = dto.CategoryId,
            AuthorId = dto.AuthorId,

        };


        var mainFileName = await dto.MainFile.SaveFileAsync(_env.WebRootPath, "assets", "image", "productImgs");
        var mainProductImageCreate = CreateProduct(mainFileName, false, true, product);
        product.ProductImgs.Add(mainProductImageCreate);

        var hoverFileName = await dto.HoverFile.SaveFileAsync(_env.WebRootPath, "assets", "image", "productImgs");
        var hoverProductImageCreate = CreateProduct(hoverFileName, true, false, product);
        product.ProductImgs.Add(hoverProductImageCreate);

        foreach (var file in dto.AdditionalFiles)
        {
            var filename = await file.SaveFileAsync(_env.WebRootPath, "assets", "image", "productImgs");
            var additionalProductImgs = CreateProduct(filename, false, false, product);
            product.ProductImgs.Add(additionalProductImgs);

        }

        await _context.Products.AddAsync(product);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");


    }
    public ProductImg CreateProduct(string url, bool isHover, bool isMain, Product product)
    {
        return new ProductImg
        {
            Url = url,
            IsMain = isMain,
            IsHover = isHover,
            Product = product
        };
    }

    public async Task<IActionResult> Update()
    {




        return RedirectToAction("Index");
    }

}

