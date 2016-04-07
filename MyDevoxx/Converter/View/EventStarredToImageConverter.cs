using System;
using Windows.UI.Xaml.Data;

namespace MyDevoxx.Converter.View
{
    public class EventStarredToImageConverter : IValueConverter
    {
        private string starredImage = "ms-appx:///Assets/star_white_filled.png";
        private string unstarredImage = "ms-appx:///Assets/star_white_empty.png";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return unstarredImage;
            }
            bool starred = (bool)value;
            if (starred)
            {
                return starredImage;
            }
            else
            {
                return unstarredImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
