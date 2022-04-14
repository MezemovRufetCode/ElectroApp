using ElectroApp.DAL;
using ElectroApp.Models;
using ElectroApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ElectroApp.Areas.ElectroManager.Controllers
{
    [Area("ElectroManager")]
    //[Authorize(Roles = ("SuperAdmin,Admin"))]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _usermanager;
        private readonly RoleManager<IdentityRole> _rolemanager;
        private readonly SignInManager<AppUser> _signinmanager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _usermanager = userManager;
            _rolemanager = roleManager;
            _signinmanager = signInManager;
            _context = context;
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid)
                return View();
            AppUser user = new AppUser
            {
                Email = register.Email,
                UserName = register.Username,
                Fullname = register.Fullname,
                IsAdmin = true
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
            await _usermanager.AddToRoleAsync(user, "Admin");
            string token = await _usermanager.GenerateEmailConfirmationTokenAsync(user);
            string link = Url.Action(nameof(VerifyEmail), "Account", new { email = user.Email, token }, Request.Scheme, Request.Host.ToString());
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("mezemovrufet2020@gmail.com", "Electro");
            mail.To.Add(new MailAddress(user.Email));
            mail.Subject = "Verify Email";
            string body = string.Empty;
            using (StreamReader reader = new StreamReader("wwwroot/assets/Template/verifyemail.html"))
            {
                body = reader.ReadToEnd();
            }
            string about = $"Welcome <strong>{user.Fullname}</strong> to Electro,please click the link in below to verify your account";
            body = body.Replace("{{link}}", link);
            mail.Body = body.Replace("{{About}}", about);
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("mezemovrufetcode@gmail.com", "Mezemov15032000Code");
            smtp.Send(mail);
            TempData["Verify"] = true;
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            AppUser user = await _usermanager.FindByEmailAsync(email);
            if (user == null) return BadRequest();
            await _usermanager.ConfirmEmailAsync(user, token);
            await _signinmanager.SignInAsync(user, true);
            TempData["Verified"] = true;
            return RedirectToAction("Index", "Dashboard");
        }
        public IActionResult Login()
        {
            return View();
        }


        [Authorize]
        public async Task<IActionResult> Edit()
        {
            AppUser user = await _usermanager.FindByNameAsync(User.Identity.Name);
            UserEditVM editedUser = new UserEditVM
            {
                Username = user.UserName,
                Email = user.Email,
                Fullname = user.Fullname
            };
            return View(editedUser);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditVM editedUser)
        {
            if (!ModelState.IsValid) return View(editedUser);
            AppUser user = await _usermanager.FindByNameAsync(User.Identity.Name);
            UserEditVM eUser = new UserEditVM
            {
                Username = user.UserName,
                Email = user.Email,
                Fullname = user.Fullname
            };
            if (user.UserName != editedUser.Username && await _usermanager.FindByNameAsync(editedUser.Username) != null)
            {
                ModelState.AddModelError("", $"{editedUser.Username} already existed");
                return View(eUser);
            }
            if (user.Email != editedUser.Email && await _usermanager.FindByEmailAsync(editedUser.Email) != null)
            {
                ModelState.AddModelError("", $"{editedUser.Email} already taken");
                return View(eUser);
            }
            if (string.IsNullOrWhiteSpace(editedUser.CurrentPassword) && string.IsNullOrEmpty(editedUser.Password) && string.IsNullOrEmpty(editedUser.ComfirmPassword))
            {
                user.UserName = editedUser.Username;
                user.Email = editedUser.Email;
                user.Fullname = editedUser.Fullname;
                await _usermanager.UpdateAsync(user);
                await _signinmanager.SignInAsync(user, true);
            }
            else
            {
                if (editedUser.CurrentPassword == null || editedUser.Password == null || editedUser.ComfirmPassword == null)
                {
                    ModelState.AddModelError("", "Fill Password Requirment");
                    return View(eUser);
                }
                user.UserName = editedUser.Username;
                user.Email = editedUser.Email;
                user.Fullname = editedUser.Fullname;
                IdentityResult resultf = await _usermanager.UpdateAsync(user);
                if (!resultf.Succeeded)
                {
                    foreach (IdentityError error in resultf.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(eUser);
                }
                IdentityResult result = await _usermanager.ChangePasswordAsync(user, editedUser.CurrentPassword, editedUser.Password);
                if (!result.Succeeded)
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(eUser);
                }
                await _signinmanager.SignInAsync(user, true);
            }
            return RedirectToAction("Index", "Dashboard");
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(AccountVM account)
        {
            AppUser user = await _usermanager.FindByEmailAsync(account.AppUser.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User is not existed");
                return View();
            }
            //return BadRequest();
            string token = await _usermanager.GeneratePasswordResetTokenAsync(user);
            string link = Url.Action(nameof(ResetPassword), "Account", new { email = user.Email, token }, Request.Scheme, Request.Host.ToString());
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("mezemovrufetcode@gmail.com", "Electro");
            mail.To.Add(new MailAddress(user.Email));
            mail.Subject = ("Reset Password");
            mail.Body = $"<a href='{link}'> Please click here to reset your password </a>";
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("mezemovrufetcode@gmail.com", "Mezemov15032000Code");
            smtp.Send(mail);
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            AppUser user = await _usermanager.FindByEmailAsync(email);
            if (user == null) return BadRequest();
            AccountVM model = new AccountVM
            {
                AppUser = user,
                Token = token
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(AccountVM account)
        {
            AppUser user = await _usermanager.FindByEmailAsync(account.AppUser.Email);
            AccountVM model = new AccountVM
            {
                AppUser = user,
                Token = account.Token
            };
            if (!ModelState.IsValid)
                return View(model);
            IdentityResult result = await _usermanager.ResetPasswordAsync(user, account.Token, account.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
            return RedirectToAction(nameof(Login));
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

            Microsoft.AspNetCore.Identity.SignInResult result = await _signinmanager.PasswordSignInAsync(user, login.Password, login.Remember, true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or password is wrong");
                return View();
            }
            return RedirectToAction("Index", "Dashboard");
        }
        public async Task<IActionResult> Logout()
        {
            await _signinmanager.SignOutAsync();
            return LocalRedirect("/electromanager/account/login");
        }

        //public async Task CreateRole()
        //{
        //    await _rolemanager.CreateAsync(new IdentityRole("SuperAdmin"));
        //    await _rolemanager.CreateAsync(new IdentityRole("Admin"));
        //    await _rolemanager.CreateAsync(new IdentityRole("Member"));
        //}
        [Authorize(Roles = "SuperAdmin")]
        public async Task CreateAdmin()
        {
            AppUser user = new AppUser
            {
                UserName = "mezemov2020",
                Email = "mezemovrufet2020@gmail.com",
                Fullname = "Rufet Mezemov"
            };
            await _usermanager.CreateAsync(user, "mezemov15032000");
            await _usermanager.AddToRoleAsync(user, "Admin");
        }

        //[Authorize(Roles ="SuperAdmin")]
        public IActionResult ShowAllUser()
        {
            List<AppUser> users = _context.Users.OrderByDescending(u => u.Id).ToList();
            return View(users);
        }

        //public async Task<IActionResult> ChangeRole(string id)
        //{
        //    if (id == null) return NotFound();
        //    AppUser appUser = await _usermanager.FindByIdAsync(id);
        //    if (appUser == null) return NotFound();
        //    string role = (await _usermanager.GetRolesAsync(appUser)).First();
        //    ViewBag.Role = role;
        //    List<IdentityRole> roles = _rolemanager.Roles.ToList();
        //    return View(roles);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ChangeRole(string id,string newRole)
        //{
        //    List<IdentityRole> roles = _rolemanager.Roles.ToList();
        //    if (id == null) return NotFound();
        //    AppUser appUser = await _usermanager.FindByIdAsync(id);
        //    if (appUser == null) return NotFound();
        //    string oldRole = (await _usermanager.GetRolesAsync(appUser)).First();
        //    if (oldRole != newRole)
        //    {
        //        IdentityResult addresult = await _usermanager.AddToRoleAsync(appUser, newRole);
        //        if (!addresult.Succeeded)
        //        {
        //            ModelState.AddModelError("", "You can not change role");
        //            return View(roles);
        //        }
        //        IdentityResult removeresult = await _usermanager.RemoveFromRoleAsync(appUser, oldRole);
        //        if (!removeresult.Succeeded)
        //        {
        //            ModelState.AddModelError("", "You can not remove role");

        //            return View(roles);
        //        }

        //        await _usermanager.UpdateAsync(appUser);
        //    }
        //    return RedirectToAction(nameof(Index));
        //}
    }
}
