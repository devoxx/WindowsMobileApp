using MyDevoxx.UserControls;
using MyDevoxx.Utils;
using MyDevoxx.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.Graphics.Display;

namespace MyDevoxx.Views
{
    public sealed partial class ScheduleView : Page
    {
        private ScheduleViewModel vm;
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        private static string SEARCH_SETTINGS = "SearchStringScheduleView";

        public ScheduleView()
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            vm = ((ScheduleViewModel)DataContext);

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

            if (vm.FilterCount > 0)
            {
                ScheduleHeader.FilterIcon = Header.FILTER_ACTIVE;
            }
            else
            {
                ScheduleHeader.FilterIcon = Header.FILTER_INACTIVE;
            }

            var searchSettings = settings.Values[SEARCH_SETTINGS];
            if (searchSettings != null && (string)searchSettings != "")
            {
                ScheduleHeader.SearchString = (string)searchSettings;
                ScheduleHeader.ShowSearch();
            }
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid header = (Grid)sender;
            UIElement arrow = header.Children.Last();
            Grid parent = (Grid)header.Parent;
            UIElement list = parent.Children.Last();
            if (list.Visibility.Equals(Visibility.Visible))
            {
                list.Visibility = Visibility.Collapsed;
                ((CompositeTransform)arrow.RenderTransform).Rotation = 0;
            }
            else
            {
                list.Visibility = Visibility.Visible;
                ((CompositeTransform)arrow.RenderTransform).Rotation = 180;
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

        private void ScheduleHeader_FilterTapped(object sender)
        {
            FilterGrid.Visibility = Visibility.Visible;
        }

        public bool IsFilterVisile()
        {
            return FilterGrid.Visibility.Equals(Visibility.Visible);
        }

        public void CloseFilter()
        {
            this.Apply();
        }

        private void Apply_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Apply();
        }

        private void Apply()
        {
            if (vm.FilterCount > 0)
            {
                ScheduleHeader.FilterIcon = Header.FILTER_ACTIVE;
            }
            else
            {
                ScheduleHeader.FilterIcon = Header.FILTER_INACTIVE;
            }
            FilterGrid.Visibility = Visibility.Collapsed;
            Messenger.Default.Send<MessageType>(MessageType.REFRESH_SCHEDULE);
        }

        private void Clear_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ScheduleHeader.FilterIcon = Header.FILTER_INACTIVE;
            FilterGrid.Visibility = Visibility.Collapsed;
            Messenger.Default.Send<MessageType>(MessageType.CLEAR_FILTER_SCHEDULE);
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
    }
}
