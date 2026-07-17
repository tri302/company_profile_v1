using company_profile.Models;
using company_profile.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace company_profile.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;

            _signInManager = signInManager;

            _roleManager = roleManager;
        }


        // ================= Change Password =================
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 1. Dapatkan user yang sedang login
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Ubah kata sandi
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                // 3. Refresh session agar user tidak logout otomatis setelah ganti password
                await _signInManager.RefreshSignInAsync(user);

                // Tambahkan pesan sukses (bisa menggunakan TempData)
                TempData["SuccessMessage"] = "Kata sandi berhasil diubah!";
                return RedirectToAction("Dashboard", "Admin");
            }

            //4.Jika gagal, tambahkan error ke ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }


            // 1. Komentari baris asli agar tidak menembak ke database
            // var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            // 2. Buat result buatan yang selalu sukses untuk keperluan demo
            //var result = Microsoft.AspNetCore.Identity.IdentityResult.Success;

            //if (result.Succeeded)
            //{
            //    // 3. Refresh session agar user tidak logout otomatis setelah ganti password
            //    await _signInManager.RefreshSignInAsync(user);

            //    // Tambahkan pesan sukses (bisa menggunakan TempData)
            //    TempData["SuccessMessage"] = "Kata sandi berhasil diubah! (Mode Demo)";
            //    return RedirectToAction("Dashboard", "Admin");
            //}

            //// 4. Jika gagal, tambahkan error ke ModelState
            //foreach (var error in result.Errors)
            //{
            //    ModelState.AddModelError(string.Empty, error.Description);
            //}

            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ================= LOGIN =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                // Masalah utama di hosting: Sesi lama belum bersih sempurna.
                // Kita pastikan sign out dulu sebelum membuat sesi baru.
                await _signInManager.SignOutAsync();

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Tambahkan ini untuk memastikan claims principal di-refresh ke versi terbaru
                    await _signInManager.RefreshSignInAsync(user);

                    return RedirectToAction("Dashboard", "Admin");
                }

                if (result.IsLockedOut)
                {
                    TempData["ErrorMessage"] = "Akun Anda terkunci sementara, silakan coba beberapa saat lagi.";
                    return View(model);
                }
            }

            TempData["ErrorMessage"] = "Email atau kata sandi Anda salah.";
            return View(model);
        }

        // ================= LOGOUT =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(
                "Login",
                "Account");
        }        
    }
}
