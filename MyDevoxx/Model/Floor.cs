using SQLite.Net.Attributes;

namespace MyDevoxx.Model
{
    public class Floor
    {
        [PrimaryKey]
        public string img { get; set; }
        public string confId { get; set; }
        public string title { get; set; }
        public string tabpos { get; set; }
        public string target { get; set; }
    }
}
