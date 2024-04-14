using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Admin.Dtos;
using Pustok.Data;
using Pustok.Enums;
using Pustok.Models;

namespace Pustok.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]

public class BrandController : Controller
{
    private readonly AppDbContext _context;

    public BrandController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var authors = await _context.Brands.Where(x => !x.SoftDelete).ToListAsync();
        return View(authors);
    }


    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(BrandCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var isExist = await _context.Brands.AnyAsync(x => x.Name.ToLower() == dto.Name.ToLower());
        if (isExist)
        {
            ModelState.AddModelError("Name", "Brand already exist");
            return View(dto);
        }
        Brand brand = new Brand() { Name = dto.Name };

        await _context.Brands.AddAsync(brand);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int id)
    {
        var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == id);

        if (brand is null)
        {
            return NotFound();

        }

        BrandUpdateDto dto = new() { Name = brand.Name };
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, BrandUpdateDto dto)
    {

        var existBrand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == id);
        if (existBrand is null)
        {
            return NotFound();
        }
        if (!ModelState.IsValid)
        {
            return View(dto);
        }


        var isExist = await _context.Brands.AnyAsync(x => x.Name.ToLower() == dto.Name.ToLower() && x.Id != id);

        if (isExist)
        {
            ModelState.AddModelError("Name", "Brand alredy exist");
            return View(dto);
        }

        existBrand.Name = dto.Name;
        _context.Brands.Update(existBrand);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == id);
        if (brand is null)
            return NotFound();

        brand.SoftDelete = true;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}

