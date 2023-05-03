namespace ElearnApp.Models
{
    public class Course:BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int SaleCount { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public ICollection<CourseImage> CourseImages { get; set; }

    }
}
