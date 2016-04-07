
using MyDevoxx.Model;
using System.Collections.Generic;

namespace MyDevoxx.Converter.Model
{
    public class TrackListConverter
    {
        public static List<Track> apply(Services.RestModel.TrackList trackList, string confId)
        {
            List<Track> result = new List<Track>();
            foreach (Services.RestModel.Track t in trackList.tracks)
            {
                Track track = new Track();
                track.confId = confId;
                track.id = t.id;
                track.dbId = confId + t.id;
                track.title = t.title;
                track.imgsrc = t.imgsrc;
                track.description = t.description;

                result.Add(track);
            }
            return result;
        }
    }
}
