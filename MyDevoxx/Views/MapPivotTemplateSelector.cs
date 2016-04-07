using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyDevoxx.Views
{
    public class MapPivotTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate MapTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item.GetType() == typeof(Geopoint))
            {
                return MapTemplate;
            }
            else
            {
                return ImageTemplate;
            }
        }
    }
}
