using MyDevoxx.Model;
using System;
using Windows.UI.Xaml.Data;

namespace MyDevoxx.Converter.View
{
    public class SelectorViewDaysLeftConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "";
            }

            Conference conference = (Conference)value;
            DateTime fromDate;
            bool isSuccessful = DateTime.TryParse(conference.fromDate, out fromDate);

            if (fromDate != null && isSuccessful)
            {
                return (int)(fromDate - DateTime.Today).TotalDays;
            }
            else {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
