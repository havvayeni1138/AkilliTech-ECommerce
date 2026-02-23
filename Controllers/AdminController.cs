using Microsoft.AspNetCore.Mvc;
using EticaretUygulamasi.Data;
using Microsoft.EntityFrameworkCore;

namespace EticaretUygulamasi.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("AdminMi") == "True")
            {
                ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
