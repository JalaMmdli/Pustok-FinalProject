using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Admin.Dtos;
using Pustok.Data;
using Pustok.Enums;
using Pustok.Extensions;
using Pustok.Models;

namespace Pustok.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]

public class SliderController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public SliderController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {

        var sliders = await _context.Sliders.ToListAsync();
        return View(sliders);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(SliderCreateDto dto)
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

        string UniqueFileName = await dto.File.SaveFileAsync(_env.WebRootPath, "assets", "image", "sliderIcons");

        Slider newSlider = new()
        {
            Title = dto.Title,
            Description = dto.Description,
            Button = dto.Button,
            ImagePath = UniqueFileName
        };
        await _context.Sliders.AddAsync(newSlider);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int id)
    {
        var slider = await _context.Sliders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (slider is null)
        {
            return NotFound();
        }

        SliderUpdateDto dto = new()
        {
            Title = slider.Title,
            Description = slider.Description,
            Button = slider.Button,
            ImagePath = slider.ImagePath
        };


        return View(dto);

    }
    [HttpPost]
    public async Task<IActionResult> Update(int id, SliderUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var existSlider = await _context.Sliders.FirstOrDefaultAsync(x => x.Id == id);

        if (existSlider is null)
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
            existSlider.ImagePath.DeleteFile(_env.WebRootPath, "assets", "image", "sliderIcons");

            var uniqueFileName = await dto.File.SaveFileAsync(_env.WebRootPath, "assets", "image", "sliderIcons");
            existSlider.ImagePath = uniqueFileName;
        }
        existSlider.Description = dto.Description;
        existSlider.Button = dto.Button;
        existSlider.Title = dto.Title;
        _context.Update(existSlider);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var slider = await _context.Sliders.FirstOrDefaultAsync(x => x.Id == id);


        if (slider is null)
        {
            return NotFound();
        }
        slider.ImagePath.DeleteFile(_env.WebRootPath, "assets", "image","sliderIcons");
        _context.Sliders.Remove(slider);


        return RedirectToAction("Index");
    }
}

