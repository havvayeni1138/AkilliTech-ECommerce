using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EticaretUygulamasi.Data;
using EticaretUygulamasi.Models;

namespace EticaretUygulamasi.Controllers
{
    public class OdemeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OdemeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Odeme/Odeme?siparisId=5
        [HttpGet]
        public async Task<IActionResult> Odeme(int siparisId)
        {
            var siparis = await _context.Siparisler
                .FirstOrDefaultAsync(s => s.SiparisId == siparisId);

            if (siparis == null)
            {
                TempData["Error"] = "Sipariş bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            return View(siparis); // ➤ Views/Odeme/Odeme.cshtml dosyasına yönlendirir
        }

        // POST: Odeme/Odeme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Odeme(int siparisId, string kartNo, string sonKullanmaTarihi, string cvv)
        {
            var siparis = await _context.Siparisler.FindAsync(siparisId);
            if (siparis == null)
            {
                TempData["Error"] = "Sipariş bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            // ⚠ Gerçek ödeme sistemi burada entegre edilmelidir.
            // Şu an örnek olarak sipariş durumu "Ödendi" yapılıyor.
            siparis.SiparisDurumu = "Ödendi";
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Ödeme başarıyla alındı. Sipariş Numaranız: {siparis.SiparisId}";
            return RedirectToAction("Index", "Siparis");
        }
    }
}
