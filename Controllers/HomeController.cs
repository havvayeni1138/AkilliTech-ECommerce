using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EticaretUygulamasi.Models;
using EticaretUygulamasi.Data;
using Microsoft.EntityFrameworkCore;

namespace EticaretUygulamasi.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // Ortak kategori listesini ViewBag'e set eden async helper metod
    private async Task SetKategorilerAsync()
    {
        ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
    }

    public async Task<IActionResult> Index()
    {
        await SetKategorilerAsync();

        var oneCikanUrunler = await _context.Urunler
            .Include(u => u.Kategori)
            .OrderByDescending(u => u.UrunId)
            .Take(8)
            .ToListAsync();

        return View(oneCikanUrunler);
    }

    public async Task<IActionResult> Kategori(int id)
    {
        await SetKategorilerAsync();

        var urunler = await _context.Urunler
            .Include(u => u.Kategori)
            .Where(u => u.KategoriId == id)
            .ToListAsync();

        var kategori = await _context.Kategoriler.FindAsync(id);
        ViewBag.KategoriAdi = kategori?.KategoriAdi;

        return View("Index", urunler);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
