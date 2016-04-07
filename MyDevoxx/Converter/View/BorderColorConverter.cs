using MyDevoxx.ViewModel;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MyDevoxx.Converter.View
{
    class BorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !value.GetType().Equals(typeof(SlotGroup)))
            {
                return new SolidColorBrush(Colors.Gray);
            }
            SlotGroup group = value as SlotGroup;
            if (DateTime.Now > group.StartDateTime && DateTime.Now < group.EndDateTime)
            {
                return new SolidColorBrush(Colors.Red);
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
