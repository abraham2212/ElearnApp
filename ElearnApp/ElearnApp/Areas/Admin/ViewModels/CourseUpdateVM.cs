using ElearnApp.Models;
using System.ComponentModel.DataAnnotations;

namespace ElearnApp.Areas.Admin.ViewModels
{
    public class CourseUpdateVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Don`t be empty")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Don`t be empty")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Don`t be empty")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Don`t be empty")]
        public int SaleCount { get; set; }

        public int AuthorId { get; set; }
        public ICollection<CourseImage> CourseImages { get; set; }
        public List<IFormFile> Photos { get; set; }
    }
}
