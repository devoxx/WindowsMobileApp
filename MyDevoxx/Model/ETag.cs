using SQLite.Net.Attributes;
using System;

namespace MyDevoxx.Model
{
    public class ETag
    {
        [PrimaryKey]
        public string url { get; set; }
        public string eTag { get; set; }
        public DateTime timeStamp { get; set; }
    }
}
