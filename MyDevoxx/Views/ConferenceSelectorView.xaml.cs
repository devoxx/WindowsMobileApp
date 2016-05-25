using MyDevoxx.Services;
using MyDevoxx.UserControls;
using MyDevoxx.ViewModel;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using FFImageLoading;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkID=390556 dokumentiert.

namespace MyDevoxx
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class ConferenceSelectorView : Page
    {
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

        private bool isLoading = false;
        private ConferenceSelectorViewModel vm { get; set; }
        private ConferenceSelector conferenceSelector { get; set; }

        public ConferenceSelectorView()
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            this.InitializeComponent();

            vm = (ConferenceSelectorViewModel)DataContext;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Conference Selector is build here to avoid strange loading issues which freezes the UI
            // I really do not like this, but I give up to find a better solution!
            conferenceSelector = new ConferenceSelector();
            ContentGrid.Children.Add(conferenceSelector);
            conferenceSelector.ItemList = vm.SelectorItemList;
            conferenceSelector.ConferenceSelected += this.conferenceSelector_ConferenceSelected;
            conferenceSelector.Height = 400;
            conferenceSelector.Width = 400;
            conferenceSelector.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            conferenceSelector.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
            conferenceSelector.Margin = new Windows.UI.Xaml.Thickness(0, 90, 0, 0);

            vm.LoadData();
        }

        private void GoButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (vm.Conference != null && vm.Conference.id != null && !isLoading)
            {
                isLoading = true;
                GoButton.Content = "Loading data ...";

                settings.Values["conferenceId"] = vm.Conference.id;
                settings.Values["cfpEndpoint"] = vm.Conference.cfpEndpoint;
                settings.Values["talkURL"] = vm.Conference.talkURL;
                settings.Values["country"] = vm.Conference.country;
                settings.Values["votingURL"] = vm.Conference.votingURL;                
                settings.Values["loadedSuccessful"] = false;

                var dataService = ServiceLocator.Current.GetInstance<IDataService>();
                dataService.CollectAll(finishedLoading);

                conferenceSelector.RotateGlobe(true);
            }
        }

        private void finishedLoading(bool successful)
        {
            conferenceSelector.RotateGlobe(false);
            if (successful)
            {
                settings.Values["loadedSuccessful"] = true;

                var nav = ServiceLocator.Current.GetInstance<INavigationService>();
                nav.NavigateTo(ViewModelLocator.ScheduleViewKey);

                vm.Cleanup();
                ContentGrid.Children.Remove(conferenceSelector);
                conferenceSelector = null;
            } else
            {
                IDialogService DialogService = ServiceLocator.Current.GetInstance<IDialogService>();
                DialogService.ShowMessage("This conference is not available yet.", "I'm sorry!");
            }

            GoButton.Content = "Go !";
            isLoading = false;                             
        }

        private void Background_ImageFailed(object sender, Windows.UI.Xaml.ExceptionRoutedEventArgs e)
        {
            if (typeof(ImageBrush).Equals(sender.GetType()))
            {
                ((ImageBrush)sender).ImageSource = new BitmapImage(new System.Uri(@"ms-appx:///Assets/FallbackData/Background.jpg"));
            }
        }

        private void conferenceSelector_ConferenceSelected(object sender, Model.Conference e)
        {
            vm.Conference = e;
        }
    }
}
