using MyDevoxx.Utils;
using MyDevoxx.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// Die Elementvorlage "Benutzersteuerelement" ist unter http://go.microsoft.com/fwlink/?LinkId=234236 dokumentiert.

namespace MyDevoxx.UserControls
{
    public sealed partial class Header : UserControl, INotifyPropertyChanged
    {
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

        private INavigationService NavigationService;

        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void FilterTappedHandler(object sender);
        public event FilterTappedHandler FilterTapped;

        public delegate void SearchTappedHandler(object sender, string searchString);
        public event SearchTappedHandler SearchTapped;

        private static string REPORT_ISSUE_URL = "https://github.com/devoxx/WindowsMobileApp/issues";

        public static string FILTER_INACTIVE = "ms-appx:///Assets/filter_empty.png";
        public static string FILTER_ACTIVE = "ms-appx:///Assets/filter_filled.png";

        private string _filterIcon = FILTER_INACTIVE;
        public string FilterIcon
        {
            get
            {
                return _filterIcon;
            }
            set
            {
                _filterIcon = value;
                NotifyPropertyChanged();
            }
        }

        private string _country;
        public string Country
        {
            get { return _country; }
            set
            {
                if (value != this._country)
                {
                    this._country = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                if (value != this._searchString)
                {
                    this._searchString = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public static readonly DependencyProperty SearchProperty =
            DependencyProperty.Register("Search", typeof(bool), typeof(NavigationBar), new PropertyMetadata(false));
        public bool Search
        {
            get { return (bool)GetValue(SearchProperty); }
            set
            {
                SetValue(SearchProperty, value);
                NotifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty FilterProperty =
        DependencyProperty.Register("Filter", typeof(bool), typeof(NavigationBar), new PropertyMetadata(false));
        public bool Filter
        {
            get { return (bool)GetValue(FilterProperty); }
            set
            {
                SetValue(FilterProperty, value);
                NotifyPropertyChanged();
            }
        }

        public Header()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;

            NavigationService = ServiceLocator.Current.GetInstance<INavigationService>();
            Messenger.Default.Register<MessageType>(this, SearchFinished);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Country = (string)settings.Values["country"];
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.NavigateTo(ViewModelLocator.AboutViewKey);
        }

        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.NavigateTo(ViewModelLocator.CreditsViewKey);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.NavigateTo(ViewModelLocator.SettingsViewKey);
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.NavigateTo(ViewModelLocator.RegisterViewKey);
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Menu_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            if (grid != null)
            {
                Flyout.ShowAttachedFlyout(grid);
            }
        }

        private void Filter_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (FilterTapped != null)
            {
                FilterTapped(this);
            }
        }

        private void SearchOpen_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ShowSearch();
        }

        private void SearchClose_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SearchGrid.Visibility = Visibility.Collapsed;
            if (SearchTapped != null)
            {
                SearchTapped(this, null);
                SearchString = "";
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //SearchGrid.Visibility = Visibility.Collapsed;
        }

        private void Search_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (SearchTapped != null)
            {
                SearchTapped(this, SearchString);
            }
        }

        private void SearchField_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key.Equals(VirtualKey.Enter))
            {
                if (SearchTapped != null)
                {
                    SearchField.IsEnabled = false;
                    SearchTapped(this, SearchString);                   
                }
            }
        }

        private void SearchFinished(MessageType messageType)
        {
            if (messageType.Equals(MessageType.SEARCH_COMPLETED))
            {
                SearchField.IsEnabled = true;
            }
        }

        public void ShowSearch()
        {
            SearchGrid.Visibility = Visibility.Visible;
            SearchField.Focus(FocusState.Keyboard);
        }

        private void VotingResults_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.NavigateTo(ViewModelLocator.VotingResultViewKey);
        }

        private async void ReportIssue_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(REPORT_ISSUE_URL));
        }
    }
}
