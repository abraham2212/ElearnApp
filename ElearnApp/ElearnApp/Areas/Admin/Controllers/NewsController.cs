using ElearnApp.Data;
using ElearnApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElearnApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NewsController : Controller
    {
        private readonly AppDbContext _context;
        public NewsController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<News> news = await _context.News
               .Include(c => c.Publisher)
               .Where(s => !s.SoftDelete)
               .ToListAsync();

            return View(news);
        }
        public async Task<IActionResult> Detail()
        {
            News news = await _context.News
                  .Include(c => c.Publisher)
                 .Where(s => !s.SoftDelete)
                 .FirstOrDefaultAsync();


            return View(news);
        }
        public IActionResult Create()
        {
            return View();
        }
    }
}
