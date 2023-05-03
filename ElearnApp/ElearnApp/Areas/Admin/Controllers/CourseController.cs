using ElearnApp.Areas.Admin.ViewModels;
using ElearnApp.Data;
using ElearnApp.Helpers;
using ElearnApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ElearnApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CourseController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Course> courses = await _context.Courses
                .Include(c => c.CourseImages)
                .Include(c => c.Author)
                .Where(s => !s.SoftDelete)
                .ToListAsync();

            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            Course course = await _context.Courses
                  .Include(c => c.CourseImages)
                  .Include(c => c.Author)
                 .Where(s => !s.SoftDelete)
                 .FirstOrDefaultAsync(c => c.Id == id);

            return View(course);
        }
        private async Task<SelectList> GetAuthorsAsync()
        {
            List<Author> authors = await _context.Authors.ToListAsync();
            return new SelectList(authors, "Id", "Fullname");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.authors = await GetAuthorsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateVM model)
        {
            try
            {
                ViewBag.authors = await GetAuthorsAsync();

                if (!ModelState.IsValid) return View();

                foreach (var photo in model.Photos)
                {
                    if (!photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View();
                    }
                    if (!photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        return View();
                    }
                }
                List<CourseImage> courseImages = new();

                foreach (var photo in model.Photos)
                {
                    CourseImage productImage = new()
                    {
                        Image = photo.CreateFile(_env, "images")
                    };
                    courseImages.Add(productImage);
                }

                courseImages.FirstOrDefault().IsMain = true;

                var convertedPrice = decimal.Parse(model.Price);
                Course newCourse = new()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = convertedPrice,
                    SaleCount = model.SaleCount,
                    AuthorId = model.AuthorId,
                    CourseImages = courseImages
                };

                _context.CourseImages.AddRange(courseImages);
                await _context.Courses.AddAsync(newCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            Course dbCourse = await _context.Courses.Include(c => c.CourseImages).FirstOrDefaultAsync(c => c.Id == id);
            if (dbCourse is null) return NotFound();

            foreach (var item in dbCourse.CourseImages)
            {
                string path = FileHelper.GetFilePath(_env.WebRootPath, "images", item.Image);
                FileHelper.DeleteFile(path);
            }
            _context.Courses.Remove(dbCourse);
            await _context.SaveChangesAsync();
            return Ok();

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();
            Course dbCourse = await _context.Courses.Include(c => c.CourseImages).FirstOrDefaultAsync(c => c.Id == id);
            if (dbCourse is null) return NotFound();
            ViewBag.authors = await GetAuthorsAsync();

            CourseUpdateVM model = new()
            {
                Id = dbCourse.Id,
                Name = dbCourse.Name,
                Description = dbCourse.Description,
                Price = dbCourse.Price,
                AuthorId = dbCourse.AuthorId,
                CourseImages = dbCourse.CourseImages,
                SaleCount = dbCourse.SaleCount
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, CourseUpdateVM model)
        {
            try
            {
                if (id is null) return BadRequest();
                Course dbCourse = await _context.Courses.AsNoTracking().Include(c => c.CourseImages).FirstOrDefaultAsync(c => c.Id == id);
                if (dbCourse is null) return NotFound();
                ViewBag.authors = await GetAuthorsAsync();

                if (!ModelState.IsValid)
                {
                    model.CourseImages = dbCourse.CourseImages;
                    return View(model);
                }

                List<CourseImage> courseImages = new();

                if (model.Photos is not null)
                {
                    foreach (var photo in model.Photos)
                    {
                        if (!photo.CheckFileType("image/"))
                        {
                            ModelState.AddModelError("Photo", "File type must be image");
                            model.CourseImages = dbCourse.CourseImages;
                            return View(model);
                        }
                        if (!photo.CheckFileSize(200))
                        {
                            ModelState.AddModelError("Photo", "Image size must be max 200kb");
                            model.CourseImages = dbCourse.CourseImages;
                            return View(model);
                        }
                    }
                    foreach (var photo in model.Photos)
                    {
                        CourseImage courseImage = new()
                        {
                            Image = photo.CreateFile(_env, "images")
                        };

                        courseImages.Add(courseImage);
                    }
                    _context.CourseImages.AddRange(courseImages);
                    dbCourse.CourseImages.FirstOrDefault().IsMain = true;
                }
                var convertedPrice = decimal.Parse(model.Price.ToString());
                Course newCourse = new()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    Price = convertedPrice,
                    SaleCount = model.SaleCount,
                    AuthorId = model.AuthorId,
                    CourseImages = courseImages.Count == 0 ? dbCourse.CourseImages : courseImages
                };
                _context.Courses.Update(newCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int? id)
        {
            try
            {
                if (id is null) return BadRequest();
                CourseImage image = await _context.CourseImages.FirstOrDefaultAsync(c => c.Id == id);
                if (image is null) return NotFound();
                var dbCourse = await _context.Courses
                    .Include(c => c.CourseImages)
                    .FirstOrDefaultAsync(c => c.CourseImages.Any(ci => ci.Id == id));

                DeleteResponse response = new();
                response.Result = false;

                if (dbCourse.CourseImages.Count > 1)
                {
                    string path = FileHelper.GetFilePath(_env.WebRootPath, "images", image.Image);
                    FileHelper.DeleteFile(path);

                    _context.CourseImages.Remove(image);
                    await _context.SaveChangesAsync();

                    response.Result = true;
                    
                }

                dbCourse.CourseImages.FirstOrDefault().IsMain = true;
                response.Id = dbCourse.CourseImages.FirstOrDefault().Id;
                await _context.SaveChangesAsync();

                return Ok(response);
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetStatus(int? id)
        {
            if (id == null) return BadRequest();
            var image = _context.CourseImages.FirstOrDefault(pi => pi.Id == id);

            if (image is null) return NotFound();

            image.IsMain = !image.IsMain;

            await _context.SaveChangesAsync();

            return Ok(image.IsMain);
        }

    }

    class DeleteResponse
    {
        public int? Id { get; set; }
        public bool Result { get; set; }
    }
}
