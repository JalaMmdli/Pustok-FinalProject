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


public class SettingController : Controller
{
    private readonly AppDbContext _context;

    public SettingController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var settings = await _context.Settings.ToListAsync();

        return View(settings);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(SettingCreateDto dto)
    {
        if (!ModelState.IsValid)
            return View();

        var isExist = await _context.Settings.AnyAsync(x => x.Key.ToLower() == dto.Key.ToLower());
        if (isExist)
        {
            ModelState.AddModelError("Key","Key is already exist");
            return View();
        }

        Setting setting = new() { Key = dto.Key, Value = dto.Value };

        await _context.Settings.AddAsync(setting);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }


    public async Task<IActionResult> Update(int id)
    {
        var setting = await _context.Settings.FirstOrDefaultAsync(x => x.Id == id);

        if (setting is null)
            return NotFound();

        SettingUpdateDto dto = new() { Key = setting.Key, Value = setting.Value };


        return View(dto);

    }
    [HttpPost]
    public async Task<IActionResult> Update(int id,SettingUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return View();

        var existSetting = await _context.Settings.FirstOrDefaultAsync(x => x.Id == id);

        if (existSetting is null)
            return View();


        existSetting.Value = dto.Value;

        _context.Settings.Update(existSetting);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");

    }
}

