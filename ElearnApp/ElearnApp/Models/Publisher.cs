namespace ElearnApp.Models
{
    public class Publisher:BaseEntity
    {
        public string Fullname { get; set; }
        public ICollection<News> News { get; set; }
    }
}
