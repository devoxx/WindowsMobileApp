using MyDevoxx.Model;
using System;
using Windows.UI.Xaml.Data;

namespace MyDevoxx.Converter.View
{
    public class EventDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "";
            }

            Event e = (Event)value;
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime fromTime = epoch.AddMilliseconds(e.fromTimeMillis);
            DateTime toTime = epoch.AddMilliseconds(e.toTimeMillis);
            if (fromTime != null && toTime != null)
            {
                return fromTime.ToString("MMMM dd, yyyy") + ", " + e.fromTime + " to " + e.toTime;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
