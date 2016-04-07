using SQLite.Net.Attributes;

namespace MyDevoxx.Model
{
    public class Track
    {
        [PrimaryKey]
        public string dbId { get; set; }

        public string id { get; set; }
        public string confId { get; set; }
        public string imgsrc { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }
}
