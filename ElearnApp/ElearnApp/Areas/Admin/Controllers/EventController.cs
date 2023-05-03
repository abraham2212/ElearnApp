using ElearnApp.Data;
using ElearnApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElearnApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EventController : Controller
    {
        private readonly AppDbContext _context;

        public EventController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Event> events = await _context.Events
                .Where(s => !s.SoftDelete)
                .ToListAsync();

            return View(events);
        }
        public async Task<IActionResult> Detail(int? id)
        {
            Event @event = await _context.Events
                .Where(s => !s.SoftDelete)
                .FirstOrDefaultAsync(e=>e.Id == id);

            return View(@event);
        }
        public IActionResult Create()
        {
            return View();
        }
    }
}
