using ElearnApp.Models;
using ElearnApp.Services.Interfaces;
using ElearnApp.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ElearnApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);

                AppUser newUser = new()
                {
                    Email = model.Email,
                    FullName = model.FullName,
                    UserName = model.Username,
                    IsRememberMe = model.IsRememberMe
                };

                var res = await _userManager.CreateAsync(newUser, model.Password);

                if (!res.Succeeded)
                {
                    foreach (var item in res.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item.Description);
                    }
                    return View(model);
                }

                string token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                string link = Url.Action(nameof(ConfirmEmail), "Account", new { userId = newUser.Id, token },
                                            Request.Scheme, Request.Host.ToString());


                string subject = "Register Confirmation";
                string html = string.Empty;

                using (StreamReader reader = new("wwwroot/templates/verify.html"))
                {
                    html = reader.ReadToEnd();
                }

                html = html.Replace("{{link}}", link);

                _emailService.Send(newUser.Email, subject, html);

                return RedirectToAction(nameof(VerifyEmail));
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }
        }
               

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId is null || token is null) return BadRequest();
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            await _userManager.ConfirmEmailAsync(user, token);

            await _signInManager.SignInAsync(user, user.IsRememberMe);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);
                var user = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
                if (user is null)
                {
                    user = await _userManager.FindByNameAsync(model.UsernameOrEmail);
                }
                if (user is null)
                {
                    ModelState.AddModelError(string.Empty, "Email or password is wrong");
                    return View(model);
                }

                var res = await _signInManager.PasswordSignInAsync(user, model.Password, model.IsRememberMe, false);
                if (!res.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Email or password is wrong");
                    return View(model);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
