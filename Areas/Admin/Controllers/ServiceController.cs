using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Admin.Dtos;
using Pustok.Data;
using Pustok.Extensions;
using Pustok.Models;

namespace Pustok.Areas.Admin.Controllers;
[Area("Admin")]
public class ServiceController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;


    public ServiceController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task< IActionResult> Index()
    {
        var services = await _context.Service.ToListAsync();
        return View(services);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ServiceCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        if (!dto.File.CheckFileType("image"))
        {
            ModelState.AddModelError("", "Invalid File");
            return View(dto);
        }
        if (!dto.File.CheckFileSize(2))
        {
            ModelState.AddModelError("", "Invalid File Size");
            return View(dto);
        }

        var isExistTitle = await _context.Service.AnyAsync(x => x.Title.ToLower() == dto.Title.ToLower());

        if (isExistTitle)
        {
            ModelState.AddModelError("", "Service already exist");
            return View(dto);
        }

        var uniqueFileName = await dto.File.SaveFileAsync(_env.WebRootPath, "assets", "image", "serviceIcons");
        Service newService = new()
        {
            Title = dto.Title,
            Description = dto.Description,
            Icon = uniqueFileName
        };
        await _context.Service.AddAsync(newService);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int id)
    {
        var service = await _context.Service.FirstOrDefaultAsync(x => x.Id == id);
        if (service is null)
        {
            return NotFound();
        }

        ServiceUpdateDto dto = new()
        {
            Title = service.Title,
            Description = service.Description,
            Icon = service.Icon,


        };
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id ,ServiceUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        var existService = await _context.Service.FirstOrDefaultAsync(x => x.Id == id);
        if (existService is null)
            return NotFound();

        if (dto.File is not null)
        {
            if (!dto.File.CheckFileType("image"))
            {
                ModelState.AddModelError("File", "Invalid File Type");
                return View(dto);
            }
            if (!dto.File.CheckFileSize(2))
            {
                ModelState.AddModelError("File", "Invalid File Size");
                return View(dto);
            }
            existService.Icon.DeleteFile(_env.WebRootPath, "assets", "image", "serviceIcons");

            var uniqueFileName = await dto.File.SaveFileAsync(_env.WebRootPath, "assets", "image", "serviceIcons");
            existService.Icon = uniqueFileName;

        }

        existService.Title = dto.Title;
        existService.Description = dto.Description;
        _context.Update(existService);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var service = await _context.Service.FirstOrDefaultAsync(x => x.Id == id);

        if (service is null)
        {
            return NotFound();
        }
        service.Icon.DeleteFile(_env.WebRootPath, "assets", "image", "sliderIcons");
        _context.Service.Remove(service);


        return RedirectToAction("Index");

    }
}

