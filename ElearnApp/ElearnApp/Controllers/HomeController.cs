using ElearnApp.Data;
using ElearnApp.Models;
using ElearnApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ElearnApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> sliders = await _context.Sliders
                .Where(s => !s.SoftDelete)
                .ToListAsync();

            IEnumerable<Course> courses = await _context.Courses
                .Include(c => c.CourseImages)
                .Include(c => c.Author)
                .Where(c => !c.SoftDelete)
                .ToListAsync();

            IEnumerable<Event> events = await _context.Events
               .Where(e => !e.SoftDelete)
               .ToListAsync();

            IEnumerable<News> news = await _context.News
                .Include(n=>n.Publisher)
               .Where(n => !n.SoftDelete)
               .ToListAsync();

            HomeVM model = new()
            {
                Sliders = sliders,
                Courses = courses,
                Events = events,
                News = news
            };
            return View(model);
        }

    }
}