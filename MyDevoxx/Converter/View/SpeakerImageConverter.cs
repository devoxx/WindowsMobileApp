using MyDevoxx.Model;
using System;
using Windows.UI.Xaml.Data;

namespace MyDevoxx.Converter.View
{
    public class SpeakerImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!value.GetType().Equals(typeof(Speaker)))
            {
                return @"ms-appx:///Assets/speaker_placeholder.png";
            }
            Speaker s = value as Speaker;
            var url = "http://78.47.43.92/" + s.confId + "/" + s.uuid + ".png";
            return url;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
