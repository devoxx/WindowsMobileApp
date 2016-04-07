using MyDevoxx.Model;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace MyDevoxx.Converter.View
{
    public class TalkCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((int)value > 1)
            {
                return value + " Talks";
            }
            return value + " Talk";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
