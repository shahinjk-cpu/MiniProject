using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShopApp.Data;
using PetShopApp.Models;
using PetShopApp.ViewModels;

namespace PetShopApp.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AppDbContext _context;

    public AccountController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? tab)
    {
        var vm = new AccountPageVM
        {
            ActiveTab = tab?.ToLower() == "register" ? "register" : "login"
        };

        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                vm.IsAuthenticated = true;
                vm.User = user;
                vm.Orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .Where(o => o.Email == user.Email)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();
            }
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AccountPageVM vm, string? returnUrl)
    {
        vm.ActiveTab = "login";

        if (string.IsNullOrWhiteSpace(vm.Login.Email) || string.IsNullOrWhiteSpace(vm.Login.Password))
        {
            ModelState.AddModelError("", "Email və şifrə daxil edilməlidir.");
            return View("Index", vm);
        }

        var user = await _userManager.FindByEmailAsync(vm.Login.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "Email və ya şifrə yanlışdır.");
            return View("Index", vm);
        }

        var result = await _signInManager.PasswordSignInAsync(user, vm.Login.Password, vm.Login.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = $"Xoş gəldiniz, {user.FullName}!";
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Email və ya şifrə yanlışdır.");
        return View("Index", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(AccountPageVM vm)
    {
        vm.ActiveTab = "register";

        if (string.IsNullOrWhiteSpace(vm.Register.FullName) ||
            string.IsNullOrWhiteSpace(vm.Register.Email) ||
            string.IsNullOrWhiteSpace(vm.Register.Password))
        {
            ModelState.AddModelError("", "Bütün məcburi sahələri doldurun.");
            return View("Index", vm);
        }

        if (vm.Register.Password != vm.Register.ConfirmPassword)
        {
            ModelState.AddModelError("", "Şifrələr bir-biri ilə uyğun gəlmir.");
            return View("Index", vm);
        }

        var existingUser = await _userManager.FindByEmailAsync(vm.Register.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("", "Bu email ünvanı ilə artıq qeydiyyatdan keçilib.");
            return View("Index", vm);
        }

        var user = new AppUser
        {
            UserName = vm.Register.Email,
            Email = vm.Register.Email,
            FullName = vm.Register.FullName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, vm.Register.Password);
        if (result.Succeeded)
        {
            // Auto sign in
            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["SuccessMessage"] = "Qeydiyyat uğurla tamamlandı! Xoş gəlmisiniz!";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View("Index", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["SuccessMessage"] = "Hesabdan uğurla çıxış edildi.";
        return RedirectToAction("Index", "Home");
    }
}
