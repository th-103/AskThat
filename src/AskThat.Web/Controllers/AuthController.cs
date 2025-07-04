using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AskThat.Domain.Entitites;
using AskThat.Domain.Interfaces;
using AskThat.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AskThat.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            ILogger<AuthController> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        [HttpGet]
        [Route("/Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [Route("/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userRepository.GetByUsernameAsync(model.Username);

                if (user == null || !user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre");
                    return View(model);
                }

                var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

                if (result == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.Name),
                    new Claim("UserId", user.UserId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddMinutes(20)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);

                _logger.LogInformation($"User {user.Username} logged in successfully");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user: {Username}", model.Username);
                ModelState.AddModelError(string.Empty, "Giriş yapılırken bir hata oluştu");
                return View(model);
            }
        }

        [HttpGet]
        [Route("/Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("/Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kullanıcı adı ve e-posta kontrolü
                if (await _userRepository.IsUsernameExistsAsync(model.Username))
                {
                    ModelState.AddModelError(nameof(model.Username), "Bu kullanıcı adı zaten kullanılıyor");
                    return View(model);
                }

                if (await _userRepository.IsEmailExistsAsync(model.Email))
                {
                    ModelState.AddModelError(nameof(model.Email), "Bu e-posta adresi zaten kullanılıyor");
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    RoleId = 1 // Default User role
                };

                user.Password = _passwordHasher.HashPassword(user, model.Password);

                await _userRepository.AddAsync(user);

                _logger.LogInformation($"New user registered: {user.Username}");

                TempData["Success"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration error for user: {Username}", model.Username);
                ModelState.AddModelError(string.Empty, "Kayıt olurken bir hata oluştu");
                return View(model);
            }
        }

        [HttpPost]
        [Route("/Logout")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation($"User {User.Identity?.Name} logged out");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}