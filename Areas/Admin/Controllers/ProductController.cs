using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Admin.Dtos;
using Pustok.Data;
using Pustok.Extensions;
using Pustok.Migrations;
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

    public async Task<IActionResult> Update(int id)
    {

        if (id < 1) return NotFound();
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.Brands = await _context.Brands.ToListAsync();
        ViewBag.Author = await _context.Authors.ToListAsync();

        var product = await _context.Products.Include(x => x.ProductImgs)
                                             .FirstOrDefaultAsync(x => x.Id == id);
        if (product == null) return NotFound();
        ProductUpdateDto dto = new()
        {
            Name = product.Name,
            ProductCode = product.ProductCode,
            RevardPoints = product.RevardPoints,
            IsStock = product.IsStock,
            Price = product.Price,
            Discount = product.Discount,
            Rating = product.Rating,
            ShortDescription = product.ShortDescription,
            LongDescription = product.LongDescription,
            BrandId = product.BrandId,
            CategoryId = product.CategoryId,
            AuthorId = product.AuthorId,
            MainFilePath = product.ProductImgs.FirstOrDefault(x => x.IsMain)?.Url ?? "",
            HoverFilePath = product.ProductImgs.FirstOrDefault(x => x.IsHover)?.Url ?? "",
            AdditionalFilePaths = product.ProductImgs.Where(x => !x.IsHover && !x.IsMain).Select(x => x.Url).ToList(),

        };

        return View(dto);

    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
    {

        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.Brands = await _context.Brands.ToListAsync();
        ViewBag.Author = await _context.Authors.ToListAsync();

        var existProduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

        if (existProduct is null)
        {
            return NotFound();
        }
        if (!ModelState.IsValid)
        {
            return View(dto);
        }


        var isExist = await _context.Products.AnyAsync(x => x.Name == dto.Name && x.Id != id);
        if (isExist)
        {
            ModelState.AddModelError("Name", "Product already exists");
            return View(dto);
        }


        var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == dto.CategoryId);

        if (!isExistCategory)
        {
            ModelState.AddModelError("CategoryId", "Category is not valid");
            return View(dto);
        }

        var isExistBrand = await _context.Brands.AnyAsync(x => x.Id == dto.BrandId);

        if (!isExistBrand)
        {
            ModelState.AddModelError("BrandId", "Brand is not valid");
            return View(dto);
        }

        var isExistAuthor = await _context.Authors.AnyAsync(x => x.Id == dto.AuthorId);

        if (!isExistAuthor)
        {
            ModelState.AddModelError("AuthorId", "Author is not valid");
            return View(dto);
        }

        if (dto.AdditionalFiles is not null)
        {
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
            if (dto.MainFile != null)
            {
                if (!dto.MainFile.CheckFileSize(2))
                {
                    ModelState.AddModelError("MainFile", "File cannot be more than 2mb");
                    return View(dto);
                }


                if (!dto.MainFile.CheckFileType("image"))
                {
                    ModelState.AddModelError("MainFile", "File must be image type!");
                    return View(dto);
                }


            }
            if (dto.HoverFile != null)
            {
                if (!dto.HoverFile.CheckFileSize(2))
                {
                    ModelState.AddModelError("HoverFile", "File cannot be more than 2mb");
                    return View(dto);
                }


                if (!dto.HoverFile.CheckFileType("image"))
                {
                    ModelState.AddModelError("HoverFile", "File must be image type!");
                    return View(dto);
                }
            }

        }


        if (dto.AdditionalFiles?.Count > 0)
        {
            foreach (var item in existProduct.ProductImgs.Where(x => !x.IsMain && !x.IsHover))
            {
                item.Url.DeleteFile(_env.WebRootPath, "assets", "image", "productImgs");

            }
            foreach (var file in dto.AdditionalFiles)
            {
                var filename = await file.SaveFileAsync(_env.WebRootPath, "assets", "image", "productImgs");
                var additionalProductImages = CreateProduct(filename, false, false, existProduct);
                existProduct.ProductImgs.Add(additionalProductImages);

            }
        }




        if (dto.MainFile is not null)
        {
            existProduct.ProductImgs.FirstOrDefault(x => x.IsMain)?.Url.DeleteFile(_env.WebRootPath, "assets", "image", "ptoductImgs");
            var mainFileName = await dto.MainFile.SaveFileAsync(_env.WebRootPath, "assets", "image", "ptoductImgs");
            var mainProductImage = CreateProduct(mainFileName, false, true, existProduct);
            existProduct.ProductImgs.Add(mainProductImage);

        }
        if (dto.HoverFile is not null)
        {
            existProduct.ProductImgs.FirstOrDefault(x => x.IsHover)?.Url.DeleteFile(_env.WebRootPath, "assets", "image", "ptoductImgs");
            var hoverFileName = await dto.HoverFile.SaveFileAsync(_env.WebRootPath, "assets", "image", "ptoductImgs");
            var hoverProductImageCreate = CreateProduct(hoverFileName, true, false, existProduct);
            existProduct.ProductImgs.Add(hoverProductImageCreate);
        }

        existProduct.Name = dto.Name;
        existProduct.ProductCode = dto.ProductCode;
        existProduct.RevardPoints = dto.RevardPoints;
        existProduct.IsStock = dto.IsStock;
        existProduct.Price = dto.Price;
        existProduct.Discount = dto.Discount;
        existProduct.Rating = dto.Rating;
        existProduct.ShortDescription = dto.ShortDescription;
        existProduct.LongDescription = dto.LongDescription;
        existProduct.BrandId = dto.BrandId;
        existProduct.CategoryId = dto.CategoryId;
        existProduct.AuthorId = dto.AuthorId;


        _context.Products.Update(existProduct);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));

    }

    public async Task<IActionResult> Detail(int id)
    {

       var existProduct= await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        if ( existProduct is null) return BadRequest();


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

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
            return NotFound();

        product.SoftDelete = true;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
