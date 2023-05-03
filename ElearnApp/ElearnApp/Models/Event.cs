namespace ElearnApp.Models
{
    public class Event:BaseEntity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime Time { get; set; }
    }
}
