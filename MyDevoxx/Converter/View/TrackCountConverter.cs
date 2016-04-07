using MyDevoxx.Model;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace MyDevoxx.Converter.View
{
    public class TrackCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(!value.GetType().Equals(typeof(List<Event>)))
            {
                return "ERROR!";
            }
            List<Event> eventlist = (List<Event>)value;
            HashSet<string> tracks = new HashSet<string>();
            foreach (Event e in eventlist)
            {
                tracks.Add(e.trackId);
            }
            return "in " + tracks.Count + " Tracks";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
