using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Models;
using Pustok.Services;
using Pustok.ViewModels;

namespace Pustok.Controllers;

public class AccountController : Controller
{

    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _emailService = emailService;
        _signInManager = signInManager;
    }

    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterVm vm)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        AppUser appUser = new()
        {
            UserName = vm.Username,
            Email = vm.Email,
            Surname = vm.Surname,
            Name = vm.Name
        };
        var result = await _userManager.CreateAsync(appUser, vm.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);


            }
            return View();
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

        string? url = Url.Action("ConfirmEmail", "Account", new { userId = appUser.Id, token = token }, HttpContext.Request.Scheme);


        _emailService.SendEmail(appUser.Email, "Confirm Email", url);


        return RedirectToAction("Login");
    }

    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();


        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            return BadRequest();


       await _signInManager.SignInAsync(user, false);

        return RedirectToAction("Index", "Home");


    }
}

