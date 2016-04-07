using System;
using Windows.UI.Xaml.Data;

namespace MyDevoxx.Converter.View
{
    public class EventTalkStarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool flag = (bool)value;
            if (flag)
            {
                return "ms-appx:///Assets/star_filled.png";
            }
            else
            {
                return "ms-appx:///Assets/star_empty.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
