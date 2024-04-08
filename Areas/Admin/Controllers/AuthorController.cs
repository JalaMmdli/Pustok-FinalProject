using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Admin.Dtos;
using Pustok.Data;
using Pustok.Models;

namespace Pustok.Areas.Admin.Controllers;
[Area("Admin")]
public class AuthorController : Controller
{
    private readonly AppDbContext _context;

    public AuthorController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var authors = await _context.Authors.Where(x => !x.SoftDelete).ToListAsync();
        return View(authors);
    }


    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(AuthorCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var isExist = await _context.Authors.AnyAsync(x => x.Name.ToLower() == dto.Name.ToLower());
        if (isExist)
        {
            ModelState.AddModelError("Name", "Author already exist");
            return View(dto);
        }
        Author author = new Author() { Name = dto.Name };

        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int id)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(x => x.Id == id);

        if (author is null)
        {
            return NotFound();

        }

        AuthorUpdateDto dto = new() { Name = author.Name };
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, AuthorUpdateDto dto)
    {

        var existAuthor = await _context.Authors.FirstOrDefaultAsync(x => x.Id == id);
        if (existAuthor is null)
        {
            return NotFound();
        }
        if (!ModelState.IsValid)
        {
            return View(dto);
        }


        var isExist = await _context.Authors.AnyAsync(x => x.Name.ToLower() == dto.Name.ToLower() && x.Id != id);

        if (isExist)
        {
            ModelState.AddModelError("Name", "Author alredy exist");
            return View(dto);
        }

        existAuthor.Name = dto.Name;
        _context.Authors.Update(existAuthor);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(x => x.Id == id);
        if (author is null)
            return NotFound();

        author.SoftDelete = true;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}


