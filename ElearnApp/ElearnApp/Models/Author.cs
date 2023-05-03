namespace ElearnApp.Models
{
    public class Author:BaseEntity
    {
        public string Fullname { get; set; }
        public string Image { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}
