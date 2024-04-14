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

public class TagController : Controller
{

    private readonly AppDbContext _context;

    public TagController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var authors = await _context.Tags.ToListAsync();
        return View(authors);
    }


    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TagCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var isExist = await _context.Tags.AnyAsync(x => x.Name.ToLower() == dto.Name.ToLower());
        if (isExist)
        {
            ModelState.AddModelError("Name", "Tag already exist");
            return View(dto);
        }
        Tag Tag = new Tag() { Name = dto.Name };

        await _context.Tags.AddAsync(Tag);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int id)
    {
        var Tag = await _context.Tags.FirstOrDefaultAsync(x => x.Id == id);

        if (Tag is null)
        {
            return NotFound();

        }

        TagUpdateDto dto = new() { Name = Tag.Name };
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, TagUpdateDto dto)
    {

        var existTag = await _context.Tags.FirstOrDefaultAsync(x => x.Id == id);
        if (existTag is null)
        {
            return NotFound();
        }
        if (!ModelState.IsValid)
        {
            return View(dto);
        }


        var isExist = await _context.Tags.AnyAsync(x => x.Name.ToLower() == dto.Name.ToLower() && x.Id != id);

        if (isExist)
        {
            ModelState.AddModelError("Name", "Tag alredy exist");
            return View(dto);
        }

        existTag.Name = dto.Name;
        _context.Tags.Update(existTag);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var Tag = await _context.Tags.FirstOrDefaultAsync(x => x.Id == id);
        if (Tag is null)
            return NotFound();

        _context.Tags.Remove(Tag);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}

