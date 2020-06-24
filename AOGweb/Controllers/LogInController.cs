using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AOGweb.Data;
using AOGweb.Models;
using AOGweb.Models.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AOGweb.Controllers
{
    public class LogInController : Controller
    {
        public IActionResult Index()
        {
            if(!User.Identity.IsAuthenticated)
                return View("Login");
            else
                return RedirectToAction(nameof(Index), "Devices");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn([Bind("Email, Password")] UserLogIn user, string returnURL = null)
        {
            if (ModelState.IsValid)
            {
                using var _context = new WebAppContext();
                var userdb = await _context.Users.SingleOrDefaultAsync(u => u.Email == user.Email);

                if (userdb == null)
                {
                    ModelState.AddModelError("Email", "Invalid email address.");
                    return View(user);
                }

                if(!MD5Crypt.VerifyMD5Hash(user.Password, userdb.Password))
                {
                    ModelState.AddModelError("Password", "Incorrect password.");
                    return View();
                }

                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userdb.ID.ToString()),
                    new Claim("FirstName", userdb.FirstName),
                    new Claim("Email", userdb.Email),
                    new Claim(ClaimTypes.Role, "User")
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (!string.IsNullOrEmpty(returnURL))
                    return Redirect(returnURL);
                else
                    return RedirectToAction(nameof(Index), "Devices");
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }
    }
}