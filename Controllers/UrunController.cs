using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EticaretUygulamasi.Data;
using EticaretUygulamasi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace EticaretUygulamasi.Controllers
{
    public class UrunController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UrunController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Listeleme
        public async Task<IActionResult> Index(string searchString, int? kategoriId, string sortOrder, decimal? minPrice, decimal? maxPrice)
        {
            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentKategori = kategoriId;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.PriceSortParm = sortOrder == "price" ? "price_desc" : "price";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";

            var urunler = _context.Urunler.Include(u => u.Kategori).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                urunler = urunler.Where(u => u.UrunAdi.Contains(searchString) || u.Aciklama.Contains(searchString));

            if (kategoriId.HasValue)
                urunler = urunler.Where(u => u.KategoriId == kategoriId);

            if (minPrice.HasValue)
                urunler = urunler.Where(u => u.Fiyat >= minPrice.Value);

            if (maxPrice.HasValue)
                urunler = urunler.Where(u => u.Fiyat <= maxPrice.Value);

            urunler = sortOrder switch
            {
                "price" => urunler.OrderBy(u => u.Fiyat),
                "price_desc" => urunler.OrderByDescending(u => u.Fiyat),
                "name" => urunler.OrderBy(u => u.UrunAdi),
                "name_desc" => urunler.OrderByDescending(u => u.UrunAdi),
                _ => urunler.OrderByDescending(u => u.UrunId)
            };

            return View(await urunler.ToListAsync());
        }

        // Detay
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var urun = await _context.Urunler
                .Include(u => u.Kategori)
                .FirstOrDefaultAsync(u => u.UrunId == id);

            if (urun == null) return NotFound();

            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
            return View(urun);
        }

        // Create GET
        public IActionResult Create()
        {
            ViewBag.Kategoriler = _context.Kategoriler.ToList();
            return View();
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UrunAdi,Fiyat,Aciklama,StokMiktari,KategoriId")] Urun urun, IFormFile resim)
        {
            ViewBag.Kategoriler = _context.Kategoriler.ToList();
            if (ModelState.IsValid)
            {
                if (resim != null && resim.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + resim.FileName;
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    await resim.CopyToAsync(fileStream);

                    urun.ResimUrl = "/images/" + uniqueFileName;
                }

                _context.Add(urun);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(urun);
        }

        // Edit GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var urun = await _context.Urunler.FindAsync(id);
            if (urun == null) return NotFound();

            ViewBag.Kategoriler = _context.Kategoriler.ToList();
            return View(urun);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UrunId,UrunAdi,Fiyat,Aciklama,StokMiktari,AktifMi,KategoriId")] Urun guncellenenUrun, IFormFile resim)
        {
            ViewBag.Kategoriler = _context.Kategoriler.ToList();
            if (!ModelState.IsValid)
                return View(guncellenenUrun);

            var mevcutUrun = await _context.Urunler.FindAsync(id);
            if (mevcutUrun == null)
                return NotFound();

            // Alanlarý tek tek güncelle
            mevcutUrun.UrunAdi = guncellenenUrun.UrunAdi;
            mevcutUrun.Fiyat = guncellenenUrun.Fiyat;
            mevcutUrun.Aciklama = guncellenenUrun.Aciklama;
            mevcutUrun.StokMiktari = guncellenenUrun.StokMiktari;
            mevcutUrun.AktifMi = guncellenenUrun.AktifMi;
            mevcutUrun.KategoriId = guncellenenUrun.KategoriId;

            // Yeni resim varsa güncelle
            if (resim != null && resim.Length > 0)
            {
                // Eski resmi sil
                if (!string.IsNullOrEmpty(mevcutUrun.ResimUrl))
                {
                    string eskiYol = Path.Combine(_webHostEnvironment.WebRootPath, mevcutUrun.ResimUrl.TrimStart('/'));
                    if (System.IO.File.Exists(eskiYol))
                        System.IO.File.Delete(eskiYol);
                }

                // Yeni resmi kaydet
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + resim.FileName;
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await resim.CopyToAsync(fileStream);

                mevcutUrun.ResimUrl = "/images/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Delete GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var urun = await _context.Urunler
                .Include(u => u.Kategori)
                .FirstOrDefaultAsync(u => u.UrunId == id);

            if (urun == null) return NotFound();

            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
            return View(urun);
        }

        // Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var urun = await _context.Urunler.FindAsync(id);
            if (urun != null)
            {
                if (!string.IsNullOrEmpty(urun.ResimUrl))
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, urun.ResimUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }

                _context.Urunler.Remove(urun);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UrunExists(int id)
        {
            return _context.Urunler.Any(e => e.UrunId == id);
        }
    }
}
