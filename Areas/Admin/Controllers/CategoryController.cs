using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Admin.Dtos;
using Pustok.Data;
using Pustok.Models;

namespace Pustok.Areas.Admin.Controllers;
[Area("Admin")]
public class CategoryController : Controller
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories.Include(x=>x.Products).ToListAsync();
        return View(categories);
    }
    public async Task<IActionResult> Create()
    {
        var categories = await _context.Categories.Where(x => x.ParentId == null).ToListAsync();
        ViewBag.Categories = categories;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateDto dto)
    {
        var categories = await _context.Categories.Where(x => x.ParentId == null).ToListAsync();
        ViewBag.Categories = categories;
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        if (dto.ParentId is not null)
        {
            var isExistCategory = await _context.Categories.AnyAsync(x => x.Id == dto.ParentId && x.ParentId == null);

            if (!isExistCategory)
            {
                ModelState.AddModelError("ParentId", "This Category is not found");
                return View(dto);
            }
        }
        var isExist = await _context.Categories.AnyAsync(x => x.Name.ToLower() == dto.Name.ToLower());

        if (isExist)
        {
            ModelState.AddModelError("Name", "category already exist");
            return View(dto);
        }


        Category category = new Category() { Name = dto.Name, ParentId = dto.ParentId };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int id)
    {
        var category = await _context.Categories.Include(x => x.Children).FirstOrDefaultAsync(x => x.Id == id);

        if (category is null)
            return NotFound();
        if (category.Children.Count > 0)
            ViewBag.Categories = new List<Category>();
        else
        {
            var categories = await _context.Categories.Where(x => x.ParentId == null && x.Id!=id).ToListAsync();
            ViewBag.Categories = categories;
        }


        CategoryUpdateDto dto = new() { Name = category.Name, ParentId = category.ParentId };

        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, CategoryUpdateDto dto)
    {
        var existCategory = await _context.Categories.Include(x=>x.Children).FirstOrDefaultAsync(x => x.Id == id);
        if (existCategory is null)
            return BadRequest();


        if (existCategory.Children.Count > 0)
            ViewBag.Categories = new List<Category>();
        else
        {
            var categories = await _context.Categories.Where(x => x.ParentId == null && x.Id != id).ToListAsync();
            ViewBag.Categories = categories;
        }

        if (!ModelState.IsValid)
            return View(dto);




        if (dto.ParentId is not null)
        {
            var isExistParent = await _context.Categories.AnyAsync(x => x.Id == dto.ParentId && x.ParentId == null);
            if (!isExistParent)
            {
                ModelState.AddModelError("ParentId", $"This category is not found {dto.ParentId}");
                return View(dto);

            }
        }


        var isExist = await _context.Categories.AnyAsync(x => x.Name.ToLower() == dto.Name.ToLower() && x.Id!=id);

        if (isExist)
        {
            ModelState.AddModelError("Name", "Name alredy exist");
            return View(dto);
        }

        existCategory.Name = dto.Name;
        existCategory.ParentId = dto.ParentId;

        _context.Categories.Update(existCategory);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if (category is null)
            return NotFound();

        category.SoftDelete = true;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
        
    }
}

