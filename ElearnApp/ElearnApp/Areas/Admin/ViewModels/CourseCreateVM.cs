using ElearnApp.Models;
using System.ComponentModel.DataAnnotations;

namespace ElearnApp.Areas.Admin.ViewModels
{
    public class CourseCreateVM
    {
        [Required(ErrorMessage = "Don`t be empty")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Don`t be empty")]
        public string Price { get; set; }

        [Required(ErrorMessage = "Don`t be empty")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Don`t be empty")]
        public int SaleCount { get; set; }

        [Required(ErrorMessage = "Don`t be empty")]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Don`t be empty")]
        public List<IFormFile> Photos { get; set; }
    }
}
