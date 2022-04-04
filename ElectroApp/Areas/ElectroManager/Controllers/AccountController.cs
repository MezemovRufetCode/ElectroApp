using ElectroApp.Models;
using ElectroApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Areas.ElectroManager.Controllers
{
    [Area("ElectroManager")]
    //[Authorize(Roles =("SuperAdmin,Admin"))]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _usermanager;
        private readonly RoleManager<IdentityRole> _rolemanager;
        private readonly SignInManager<AppUser> _signinresult;

        public AccountController(UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager,SignInManager<AppUser> signInResult)
        {
            _usermanager = userManager;
            _rolemanager = roleManager;
            _signinresult = signInResult;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid)
                return View();
            AppUser user = await _usermanager.FindByEmailAsync(login.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email or password is wrong");
                return View();
            }
            if (!user.IsAdmin)
            {
                ModelState.AddModelError("", "Email or password is wrong");
                return View();
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await _signinresult.PasswordSignInAsync(user, login.Password, login.Remember, true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or password is wrong");
                return View();
            }
            return RedirectToAction("Index", "Product");
        }

        //public async Task CreateRole()
        //{
        //    await _rolemanager.CreateAsync(new IdentityRole("SuperAdmin"));
        //    await _rolemanager.CreateAsync(new IdentityRole("Admin"));
        //    await _rolemanager.CreateAsync(new IdentityRole("Member"));
        //}
        public async Task CreateAdmin()
        {
            AppUser user = new AppUser
            {
                UserName = "Admin2",
                Email = "mezemovadmin@gmail.com",
                Fullname = "Admin Mezemov"
            };
            await _usermanager.CreateAsync(user, "mezemov15032000");
            await _usermanager.AddToRoleAsync(user, "Admin");
        }
    }
}
