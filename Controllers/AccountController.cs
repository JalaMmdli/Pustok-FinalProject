using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.Data;
using Pustok.Enums;
using Pustok.Models;
using Pustok.Services;
using Pustok.ViewModels;

namespace Pustok.Controllers;

public class AccountController : Controller
{

    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _context;

    public AccountController(UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
    {
        _userManager = userManager;
        _emailService = emailService;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = context;
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

        await _userManager.AddToRoleAsync(appUser, Roles.Member.ToString());

        await TransferBasket(appUser);
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

        await TransferBasket(user);

        return RedirectToAction("Index", "Home");


    }

    public IActionResult Login()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Login(LoginVm loginVm)
    {
        if (!ModelState.IsValid) return View(loginVm);

        var existUser = await _userManager.FindByEmailAsync(loginVm.Email);
        if (existUser == null)
        {
            ModelState.AddModelError("", "Invalid Credentials");
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(existUser, loginVm.Password, loginVm.RememberMe, true);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid Credentials");
            return View();
        }

        await TransferBasket(existUser);

        var role = await _userManager.IsInRoleAsync(existUser, Roles.Admin.ToString());
        if (role)
            return RedirectToAction("Index", "Dashboard", new { Area = "Admin" });
        return RedirectToAction("Index", "Home");

    }

    private async Task TransferBasket(AppUser existUser)
    {
        var basketItems = _getBasket();

        foreach (var item in basketItems)
        {
            var isExistBasket = await _context.BasketItems.AnyAsync(x => x.ProductId == item.ProductId && x.AppUserId == existUser.Id);

            if (!isExistBasket)
            {
                BasketItem basketItem = new() { ProductId = item.ProductId, AppUserId = existUser.Id , Count=item.Count};
                await _context.BasketItems.AddAsync(basketItem);
            }

        }
        await _context.SaveChangesAsync();
    }

    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }


    //public async Task<IActionResult> CreateRole()
    //{
    //    foreach (var role in Enum.GetValues(typeof(Roles)))
    //    {
    //        await _roleManager.CreateAsync(new IdentityRole
    //        {
    //            Id = Guid.NewGuid().ToString(),
    //            Name = role.ToString(),
    //        });
    //    }
    //    return RedirectToAction("Index", "Home");
    //}


    private List<BasketItem> _getBasket()
    {
        List<BasketItem> basketItems = new();
        if (Request.Cookies["basket"] != null)
        {
            basketItems = JsonConvert.DeserializeObject<List<BasketItem>>(Request.Cookies["basket"]) ?? new();
        }

        return basketItems;
    }
}

