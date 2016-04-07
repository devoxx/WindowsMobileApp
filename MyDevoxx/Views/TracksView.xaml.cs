using MyDevoxx.UserControls;
using MyDevoxx.ViewModel;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Messaging;
using MyDevoxx.Utils;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace MyDevoxx.Views
{
    public sealed partial class TracksView : Page
    {
        private TracksViewModel vm;

        public TracksView()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            vm = (TracksViewModel)DataContext;
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
            vm.LoadData();

            if (vm.ChoosenTrackFilter.Count + vm.ChoosenDayFilter.Count > 0)
            {
                TracksHeader.FilterIcon = Header.FILTER_ACTIVE;
            }
            else
            {
                TracksHeader.FilterIcon = Header.FILTER_INACTIVE;
            }
        }

        private void EventTalkControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!typeof(EventTalkControl).Equals(sender.GetType()))
            {
                return;
            }
            var nav = ServiceLocator.Current.GetInstance<INavigationService>();
            nav.NavigateTo(ViewModelLocator.TalkDetailsViewKey, ((EventTalkControl)sender).DataContext);
        }

        private void TacksHeader_FilterTapped(object sender)
        {
            FilterGrid.Visibility = Visibility.Visible;
        }

        private void TrackFilterGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (TrackFilterList.Visibility.Equals(Visibility.Collapsed))
            {
                TrackFilterList.Visibility = Visibility.Visible;
            }
            else
            {
                TrackFilterList.Visibility = Visibility.Collapsed;
            }
        }

        private void DayFilterGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (DayFilterList.Visibility.Equals(Visibility.Collapsed))
            {
                DayFilterList.Visibility = Visibility.Visible;
            }
            else
            {
                DayFilterList.Visibility = Visibility.Collapsed;
            }
        }

        private void Apply_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Apply();
        }

        public bool IsFilterVisile()
        {
            return FilterGrid.Visibility.Equals(Visibility.Visible);
        }

        public void CloseFilter()
        {
            this.Apply();
        }

        private void Apply()
        {
            if (vm.ChoosenTrackFilter.Count + vm.ChoosenDayFilter.Count > 0)
            {
                TracksHeader.FilterIcon = Header.FILTER_ACTIVE;
            }
            else
            {
                TracksHeader.FilterIcon = Header.FILTER_INACTIVE;
            }
            FilterGrid.Visibility = Visibility.Collapsed;
            Messenger.Default.Send<MessageType>(MessageType.REFRESH_TRACKS);
        }

        private void Clear_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TracksHeader.FilterIcon = Header.FILTER_INACTIVE;
            FilterGrid.Visibility = Visibility.Collapsed;
            Messenger.Default.Send<MessageType>(MessageType.CLEAR_FILTER_TRACKS);
        }
    }
}
