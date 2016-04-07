using MyDevoxx.Model;
using System;
using Windows.UI.Xaml.Data;

namespace MyDevoxx.Converter.View
{
    public class SelectorViewDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "";
            }

            Conference conference = (Conference)value;
            DateTime fromDate, toDate;
            bool isSuccessfulFrom = DateTime.TryParse(conference.fromDate, out fromDate);
            bool isSuccessfulTo = DateTime.TryParse(conference.toDate, out toDate);
            if (fromDate != null && toDate != null && isSuccessfulFrom && isSuccessfulTo)
            {
                return "from " + fromDate.Day + " to " + toDate.ToString("dd/MM/yyyy");
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
