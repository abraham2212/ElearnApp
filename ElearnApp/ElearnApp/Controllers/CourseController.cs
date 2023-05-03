using ElearnApp.Areas.Admin.Controllers;
using ElearnApp.Data;
using ElearnApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ElearnApp.Controllers
{
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;
        public CourseController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            int count  = await _context.Courses
                .Where(c => !c.SoftDelete)
                .CountAsync();
            ViewBag.Count = count;

            IEnumerable<Course> courses = await _context.Courses
                .Include(c => c.CourseImages)
                .Include(c => c.Author)
                .Where(c => !c.SoftDelete)
                .Take(3)
                .ToListAsync();

            return View(courses);
        }
        public async Task<IActionResult> LoadMore(int skip)
        {
            IEnumerable<Course> courses = await _context.Courses
                .Include(c=>c.CourseImages)
                .Include(c => c.Author)
                .Where(c => !c.SoftDelete)
                .Skip(skip)
                .Take(3)
                .ToListAsync();

            return PartialView("_CoursePartial", courses);
        }
    }
}
