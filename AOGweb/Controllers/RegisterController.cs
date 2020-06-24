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
    public class RegisterController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("FirstName, LastName, Email, Password, ConfirmPassword")] UserRegister newUser, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                using var _context = new WebAppContext();
                var userdb = await _context.Users.SingleOrDefaultAsync(u => u.Email == newUser.Email);

                //email already exists
                if (userdb != null)
                {
                    ModelState.AddModelError("Email", "Email is already taken");
                    return View();
                }

                User user = new User()
                {
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    Email = newUser.Email,
                    Password = MD5Crypt.GetMD5Hash(newUser.Password),
                    CreatedAt = DateTime.Now
                };
                _context.Users.Add(user);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "LogIn", new { returnUrl });
                }
                catch (DbUpdateException ex)
                {
                    Debug.WriteLine(ex.Message);
                    ModelState.AddModelError("", "There was an internal server error. Please try again later.");
                    return View();
                }
            }
            return View();
        }
    }
}