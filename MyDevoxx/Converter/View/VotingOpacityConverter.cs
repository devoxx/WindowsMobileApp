using System;
using Windows.UI.Xaml.Data;

namespace MyDevoxx.Converter.View
{
    public class VotingOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int rating = (int)value;
            int param = Int32.Parse((string)parameter);
            if (param <= rating)
            {
                return 1;
            }
            else
            {
                return 0.5;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
