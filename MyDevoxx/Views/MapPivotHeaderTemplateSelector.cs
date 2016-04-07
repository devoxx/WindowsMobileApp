using MyDevoxx.UserControls;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;

namespace MyDevoxx.Views
{
    public class MapPivotHeaderTemplateSelector : TemplateSelector
    {
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate MapTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
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
