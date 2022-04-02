using ElectroApp.Models;
using ElectroApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _usermanager;

        public AccountController(UserManager<AppUser> userManager)
        {
            _usermanager = userManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid)
                return View();
            AppUser user = new AppUser
            {
                Email = register.Email,
                UserName = register.Username,
                Fullname = register.Fullname
            };
            if (!register.Terms)
            {
                ModelState.AddModelError("Terms", "Please agree to all the terms and conditions before registration.");
                return View();
            }
            IdentityResult result = await _usermanager.CreateAsync(user, register.Password);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            await _usermanager.AddToRoleAsync(user, "Member");
            return RedirectToAction("Index","Home");
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}
