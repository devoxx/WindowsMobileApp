using MyDevoxx.ViewModel;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyDevoxx.Views
{
    public sealed partial class MapView : Page
    {
        public MapView()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            var o = ApplicationView.GetForCurrentView().Orientation;
            if (o.Equals(ApplicationViewOrientation.Landscape))
            {
                VisualStateManager.GoToState(this, "Landscape", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Portrait", true);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ((MapViewModel)DataContext).LoadData();
        }
    }
}
