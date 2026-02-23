using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EticaretUygulamasi.Data;
using EticaretUygulamasi.Models;

namespace EticaretUygulamasi.Controllers
{
    public class SepetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SepetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Yardımcı metod: ViewBag.Kategoriler'i async olarak set et
        private async Task SetKategorilerAsync()
        {
            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
        }

        public async Task<IActionResult> Index()
        {
            await SetKategorilerAsync();

            var sessionId = HttpContext.Session.GetString("SessionId");
            if (string.IsNullOrEmpty(sessionId))
            {
                // Sepet session bazlı, session yoksa sepet yok
                return View(new List<SepetUrunu>()); // Boş liste gönder
            }

            try
            {
                var sepetUrunleri = await _context.SepetUrunleri
                    .Include(su => su.Urun)
                    .ThenInclude(u => u.Kategori)
                    .Where(su => su.SessionId == sessionId)
                    .ToListAsync();

                return View(sepetUrunleri);
            }
            catch (Exception)
            {
                TempData["Error"] = "Sepet bilgileri yüklenirken bir hata oluştu.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int urunId, int miktar)
        {
            await SetKategorilerAsync();

            try
            {
                var sessionId = HttpContext.Session.GetString("SessionId");
                if (string.IsNullOrEmpty(sessionId))
                {
                    sessionId = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("SessionId", sessionId);
                }

                var urun = await _context.Urunler.FindAsync(urunId);
                if (urun == null)
                {
                    TempData["Error"] = "Ürün bulunamadı.";
                    return RedirectToAction("Index", "Urun");
                }

                if (urun.StokMiktari < miktar)
                {
                    TempData["Error"] = "Yeterli stok bulunmamaktadır.";
                    return RedirectToAction("Details", "Urun", new { id = urunId });
                }

                var sepetUrunu = await _context.SepetUrunleri
                    .FirstOrDefaultAsync(su => su.SessionId == sessionId && su.UrunId == urunId);

                if (sepetUrunu != null)
                {
                    sepetUrunu.Miktar += miktar;
                }
                else
                {
                    sepetUrunu = new SepetUrunu
                    {
                        SessionId = sessionId,
                        UrunId = urunId,
                        Miktar = miktar,
                        EklenmeTarihi = DateTime.Now
                    };
                    _context.SepetUrunleri.Add(sepetUrunu);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"{urun.UrunAdi} sepete eklendi.";
                return RedirectToAction("Index", "Sepet");
            }
            catch (Exception)
            {
                TempData["Error"] = "Ürün sepete eklenirken bir hata oluştu.";
                return RedirectToAction("Index", "Urun");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int sepetUrunuId, int miktar)
        {
            await SetKategorilerAsync();

            try
            {
                var sessionId = HttpContext.Session.GetString("SessionId");
                if (string.IsNullOrEmpty(sessionId))
                {
                    return RedirectToAction("Index", "Home");
                }

                var sepetUrunu = await _context.SepetUrunleri
                    .Include(su => su.Urun)
                    .FirstOrDefaultAsync(su => su.SepetUrunuId == sepetUrunuId && su.SessionId == sessionId);

                if (sepetUrunu == null)
                {
                    TempData["Error"] = "Sepet ürünü bulunamadı.";
                    return RedirectToAction("Index", "Sepet");
                }

                if (miktar <= 0)
                {
                    return await RemoveFromCart(sepetUrunuId);
                }

                if (sepetUrunu.Urun.StokMiktari < miktar)
                {
                    TempData["Error"] = "Yeterli stok bulunmamaktadır.";
                    return RedirectToAction("Index", "Sepet");
                }

                sepetUrunu.Miktar = miktar;
                await _context.SaveChangesAsync();

                TempData["Success"] = $"{sepetUrunu.Urun.UrunAdi} miktarı güncellendi.";
                return RedirectToAction("Index", "Sepet");
            }
            catch (Exception)
            {
                TempData["Error"] = "Ürün miktarı güncellenirken bir hata oluştu.";
                return RedirectToAction("Index", "Sepet");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int sepetUrunuId)
        {
            await SetKategorilerAsync();

            try
            {
                var sessionId = HttpContext.Session.GetString("SessionId");
                if (string.IsNullOrEmpty(sessionId))
                {
                    return RedirectToAction("Index", "Home");
                }

                var sepetUrunu = await _context.SepetUrunleri
                    .Include(su => su.Urun)
                    .FirstOrDefaultAsync(su => su.SepetUrunuId == sepetUrunuId && su.SessionId == sessionId);

                if (sepetUrunu == null)
                {
                    TempData["Error"] = "Sepet ürünü bulunamadı.";
                    return RedirectToAction("Index", "Sepet");
                }

                string urunAdi = sepetUrunu.Urun.UrunAdi;

                _context.SepetUrunleri.Remove(sepetUrunu);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"{urunAdi} sepetten kaldırıldı.";
                return RedirectToAction("Index", "Sepet");
            }
            catch (Exception)
            {
                TempData["Error"] = "Ürün sepetten kaldırılırken bir hata oluştu.";
                return RedirectToAction("Index", "Sepet");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            await SetKategorilerAsync();

            try
            {
                var sessionId = HttpContext.Session.GetString("SessionId");
                if (string.IsNullOrEmpty(sessionId))
                {
                    return RedirectToAction("Index", "Home");
                }

                var sepetUrunleri = await _context.SepetUrunleri
                    .Where(su => su.SessionId == sessionId)
                    .ToListAsync();

                _context.SepetUrunleri.RemoveRange(sepetUrunleri);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Sepetiniz temizlendi.";
                return RedirectToAction("Index", "Sepet");
            }
            catch (Exception)
            {
                TempData["Error"] = "Sepet temizlenirken bir hata oluştu.";
                return RedirectToAction("Index", "Sepet");
            }
        }
    }
}
