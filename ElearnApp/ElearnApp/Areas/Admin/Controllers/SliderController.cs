using ElearnApp.Data;
using ElearnApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElearnApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;

        public SliderController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            Slider slider = await _context.Sliders
                .Where(s => !s.SoftDelete)
                .FirstOrDefaultAsync();

            return View(slider);
        }
        public async Task<IActionResult> Detail()
        {
            Slider slider = await _context.Sliders
                .Where(s => !s.SoftDelete)
                .FirstOrDefaultAsync();

            return View(slider);
        }
        public IActionResult Create() 
        {
            return View();
        }
    }
}
