using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EticaretUygulamasi.Data;
using EticaretUygulamasi.Models;
using BCrypt.Net;

namespace EticaretUygulamasi.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KullaniciController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Yardımcı metod: ViewBag.Kategoriler'i async olarak set et
        private async Task SetKategorilerAsync()
        {
            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
        }

        // GET: Kullanici/Register
        public async Task<IActionResult> Register()
        {
            await SetKategorilerAsync();
            return View();
        }

        // POST: Kullanici/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Ad,Soyad,Email,Sifre,Telefon,Adres")] Kullanici kullanici)
        {
            await SetKategorilerAsync();

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(kullanici.Sifre) || !kullanici.Sifre.All(char.IsDigit) || kullanici.Sifre.Length < 6)
                {
                    ModelState.AddModelError("Sifre", "Şifre en az 6 haneli ve sadece rakamlardan oluşmalıdır.");
                    return View(kullanici);
                }

                bool emailKullaniliyor = await _context.Kullanicilar.AnyAsync(k => k.Email == kullanici.Email);
                if (emailKullaniliyor)
                {
                    ModelState.AddModelError("Email", "Bu e-posta adresi zaten kullanılıyor.");
                    return View(kullanici);
                }

                kullanici.Sifre = BCrypt.Net.BCrypt.HashPassword(kullanici.Sifre);

                _context.Add(kullanici);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(kullanici);
        }

        // GET: Kullanici/Login
        public async Task<IActionResult> Login()
        {
            await SetKategorilerAsync();
            return View();
        }

        // POST: Kullanici/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string sifre)
        {
            await SetKategorilerAsync();

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.Email == email);

            if (kullanici == null || !BCrypt.Net.BCrypt.Verify(sifre, kullanici.Sifre))
            {
                ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
                return View();
            }

            HttpContext.Session.SetInt32("KullaniciId", kullanici.KullaniciId);
            HttpContext.Session.SetString("KullaniciAdi", kullanici.Ad ?? "");
            HttpContext.Session.SetString("KullaniciSoyadi", kullanici.Soyad ?? "");
            HttpContext.Session.SetString("KullaniciEmail", kullanici.Email ?? "");
            HttpContext.Session.SetString("AdminMi", kullanici.AdminMi.ToString());

            if (kullanici.AdminMi)
            {
                return RedirectToAction("Index", "Admin");
            }
            return RedirectToAction("Index", "Urun");
        }

        // GET: Kullanici/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: Kullanici/Profile
        public async Task<IActionResult> Profile()
        {
            await SetKategorilerAsync();

            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login");
            }

            var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId);
            if (kullanici == null)
            {
                return NotFound();
            }

            return View(kullanici);
        }

        // POST: Kullanici/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(int id, [Bind("KullaniciId,Ad,Soyad,Email,Telefon,Adres")] Kullanici kullanici)
        {
            await SetKategorilerAsync();

            if (id != kullanici.KullaniciId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingKullanici = await _context.Kullanicilar.FindAsync(id);
                    if (existingKullanici == null)
                    {
                        return NotFound();
                    }

                    // Şifre ve AdminMi bilgisi korunur
                    kullanici.Sifre = existingKullanici.Sifre;
                    kullanici.AdminMi = existingKullanici.AdminMi;

                    _context.Entry(existingKullanici).CurrentValues.SetValues(kullanici);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KullaniciExists(kullanici.KullaniciId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Profile));
            }
            return View(kullanici);
        }

        private bool KullaniciExists(int id)
        {
            return _context.Kullanicilar.Any(e => e.KullaniciId == id);
        }
    }
}
