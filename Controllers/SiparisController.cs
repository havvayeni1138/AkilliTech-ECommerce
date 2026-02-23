using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EticaretUygulamasi.Data;
using EticaretUygulamasi.Models;

namespace EticaretUygulamasi.Controllers
{
    public class SiparisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SiparisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Yardımcı metod: ViewBag.Kategoriler'i set et
        private void SetKategoriler()
        {
            ViewBag.Kategoriler = _context.Kategoriler.ToList();
        }

        // GET: Siparis
        public async Task<IActionResult> Index()
        {
            SetKategoriler();

            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Kullanici");
            }

            var siparisler = await _context.Siparisler
                .Where(s => s.KullaniciId == kullaniciId.Value)
                .Include(s => s.SiparisDetaylar!)
                .ThenInclude(sd => sd.Urun)
                .OrderByDescending(s => s.SiparisTarihi)
                .ToListAsync();

            return View(siparisler);
        }

        // GET: Siparis/Tamamla
        public IActionResult Tamamla()
        {
            SetKategoriler();

            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Kullanici");
            }

            return View();
        }

        // POST: Siparis/Tamamla
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tamamla([Bind("Adres,Telefon")] Siparis siparis)
        {
            SetKategoriler();

            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Kullanici");
            }

            if (ModelState.IsValid)
            {
                siparis.KullaniciId = kullaniciId.Value;
                siparis.SiparisTarihi = DateTime.Now;
                siparis.SiparisDurumu = "Beklemede";

                var sepet = await _context.Sepetler
                    .Include(s => s.SepetDetaylar!)
                    .ThenInclude(sd => sd.Urun)
                    .FirstOrDefaultAsync(s => s.KullaniciId == kullaniciId);

                if (sepet == null || sepet.SepetDetaylar == null || !sepet.SepetDetaylar.Any())
                {
                    ModelState.AddModelError("", "Sepetiniz boş!");
                    return View(siparis);
                }

                siparis.ToplamTutar = sepet.SepetDetaylar.Sum(sd => sd.Urun.Fiyat * sd.Miktar);

                _context.Add(siparis);
                await _context.SaveChangesAsync();

                foreach (var detay in sepet.SepetDetaylar)
                {
                    var siparisDetay = new SiparisDetay
                    {
                        SiparisId = siparis.SiparisId,
                        UrunId = detay.UrunId,
                        Miktar = detay.Miktar,
                        BirimFiyat = detay.Urun.Fiyat,
                        ToplamFiyat = detay.Urun.Fiyat * detay.Miktar
                    };
                    _context.Add(siparisDetay);
                }

                _context.SepetDetaylar.RemoveRange(sepet.SepetDetaylar);
                await _context.SaveChangesAsync();

                // Ödeme ekranına yönlendir
                return RedirectToAction("Odeme", "Odeme", new { siparisId = siparis.SiparisId });
            }

            return View(siparis);
        }

        // GET: Siparis/Detay/5
        public async Task<IActionResult> Detay(int id)
        {
            SetKategoriler();

            var siparis = await _context.Siparisler
                .Include(s => s.SiparisDetaylar!)
                .ThenInclude(sd => sd.Urun)
                .FirstOrDefaultAsync(s => s.SiparisId == id);

            if (siparis == null)
            {
                return NotFound();
            }

            return View(siparis);
        }
    }
}
