namespace ElearnApp.Models
{
    public class News :BaseEntity
    {
        public string Image { get; set; }
        public DateTime Time { get; set; }
        public string Title { get; set; }
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }
    }
}

