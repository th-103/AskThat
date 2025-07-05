using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AskThat.Application.Interfaces;
using AskThat.Domain.Entitites;
using AskThat.Domain.Interfaces;
using AskThat.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AskThat.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Check if username exists
                if (await _userService.IsUsernameTakenAsync(model.Username))
                {
                    ModelState.AddModelError("Username", "Username is already taken.");
                    return View(model);
                }

                // Check if email exists
                if (await _userService.IsEmailTakenAsync(model.Email))
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(model);
                }

                // Register user
                var user = await _userService.RegisterUserAsync(model.Username, model.Email, model.Password);

                _logger.LogInformation("User {Username} registered successfully", model.Username);

                // Auto-login after registration
                await SignInUserAsync(user);

                TempData["SuccessMessage"] = "Registration successful! Welcome to AskThat.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for {Username}", model.Username);
                ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Find user by email
                var user = await _userService.GetByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    ModelState.AddModelError("", "Your account has been deactivated. Please contact support.");
                    return View(model);
                }

                // Verify password
                if (!await _userService.CheckPasswordAsync(user, model.Password))
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
                }

                // Sign in user
                await SignInUserAsync(user, model.RememberMe);

                _logger.LogInformation("User {Username} logged in successfully", user.Username);

                // Redirect to return URL or home
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", model.Email);
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out");
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUserAsync(Domain.Entitites.User user, bool rememberMe = false)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.Name)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
        }
    }
}