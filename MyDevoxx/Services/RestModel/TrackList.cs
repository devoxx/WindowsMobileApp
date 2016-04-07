using System.Collections.Generic;

namespace MyDevoxx.Services.RestModel
{
    public class TrackList
    {
        public string content { get; set; }
        public List<Track> tracks { get; set; }
    }
}
