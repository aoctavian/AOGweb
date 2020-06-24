using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AOGweb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AOGweb.Models.User;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace AOGweb.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Update()
        {
            using var _context = new WebAppContext();
            var user = await _context.Users.FindAsync(int.Parse(User.Identity.Name));
            TempData["user"] = user;
            TempData["userPassword"] = new UserPassword() { ID = user.ID, OldPassword = user.Password };
            return View("Update");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAccount([Bind("ID, FirstName, LastName, Email, Password")] User user)
        {
            if (ModelState.IsValid)
            {
                using var _context = new WebAppContext();
                var me = await _context.Users.FindAsync(int.Parse(User.Identity.Name));

                if (me.Password != user.Password)
                {
                    ModelState.AddModelError("Password", "Incorrect account password.");
                    return View(nameof(Update));
                }

                if (me.Email != user.Email)
                {
                    var userWithEmail = await _context.Users.SingleOrDefaultAsync(u => u.Email == user.Email);
                    if (userWithEmail != null)
                    {
                        ModelState.AddModelError("Email", "Email is already taken.");
                        return View(nameof(Update));
                    }
                }
                me.FirstName = user.FirstName;
                me.LastName = user.LastName;
                me.Email = user.Email;
                try
                {
                    await _context.SaveChangesAsync();
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, me.ID.ToString()),
                        new Claim("FirstName", me.FirstName),
                        new Claim("Email", me.Email),
                        new Claim("Password", me.Password),
                        new Claim(ClaimTypes.Role, "User")
                    }, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    Debug.WriteLine(ex.Message);
                    ModelState.AddModelError("", "There was an internal server error. Please try again later.");
                    return View(nameof(Update));
                }
            }
            return View(nameof(Update));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword([Bind("ID, OldPassword, NewPassword, NewConfirmPassword")] UserPassword user)
        {
            if (ModelState.IsValid)
            {
                using var _context = new WebAppContext();
                var me = await _context.Users.FindAsync(int.Parse(User.Identity.Name));

                if(me.Password != user.OldPassword)
                {
                    ModelState.AddModelError("OldPassword", "Incorrect account old password.");
                    return View(nameof(Update));
                }

                if (me.Password == user.NewPassword)
                {
                    ModelState.AddModelError("NewPassword", "Old and new passwords are the same.");
                    return View(nameof(Update));
                }

                me.Password = user.NewPassword;
                try
                {
                    await _context.SaveChangesAsync();
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, me.ID.ToString()),
                        new Claim("FirstName", me.FirstName),
                        new Claim("Email", me.Email),
                        new Claim("Password", me.Password),
                        new Claim(ClaimTypes.Role, "User")
                    }, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    Debug.WriteLine(ex.Message);
                    ModelState.AddModelError("", "There was an internal server error. Please try again later.");
                    return View(nameof(Update));
                }
            }
            return View(nameof(Update));
        }
    }
}