using MyDevoxx.Model;
using MyDevoxx.ViewModel;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;

namespace MyDevoxx.Views
{
    public sealed partial class TalkDetailsView : Page
    {
        private TalkDetailsViewModel vm;
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        private static string USERID = "userId";

        public TalkDetailsView()
        {
            this.InitializeComponent();
            vm = (TalkDetailsViewModel)DataContext;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            vm.LoadData((Event)e.Parameter);
        }

        private void BackImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private void Speaker_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!typeof(Grid).Equals(sender.GetType()))
            {
                return;
            }
            var nav = ServiceLocator.Current.GetInstance<INavigationService>();
            nav.NavigateTo(ViewModelLocator.SpeakerDetailsViewKey, ((Grid)sender).Tag);
        }

        private void Save_Tapped(object sender, TappedRoutedEventArgs e)
        {
            vm.SaveNote();
            NoteGrid.Visibility = Visibility.Collapsed;
        }

        private void EditNote_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NoteGrid.Visibility = Visibility.Visible;
        }

        private void Vote_Tapped(object sender, TappedRoutedEventArgs e)
        {
            VotingGrid.Visibility = Visibility.Collapsed;
            vm.SendVote();
        }

        private void Cancel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            VotingGrid.Visibility = Visibility.Collapsed;
        }

        private void Rating_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image image = (Image)sender;
            int rating = Int32.Parse((string)image.Tag);
            vm.Vote.rating = rating;
        }

        private void OpenRating_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string confId = (string)settings.Values["conferenceId"];
            string user = (string)settings.Values[USERID + confId];

            if (string.IsNullOrWhiteSpace(user))
            {
                var nav = ServiceLocator.Current.GetInstance<INavigationService>();
                nav.NavigateTo(ViewModelLocator.RegisterViewKey);
            }
            else
            {
                VotingGrid.Visibility = Visibility.Visible;
            }
        }

        private void VotingImage_Failed(object sender, ExceptionRoutedEventArgs e)
        {
            ((Image)sender).Source = new BitmapImage(new Uri(@"ms-appx:///Assets/heart_filled.png"));
        }

        public bool IsVotingVisile()
        {
            return VotingGrid.Visibility.Equals(Visibility.Visible);
        }

        public void CloseVotingGrid()
        {
            VotingGrid.Visibility = Visibility.Collapsed;
        }

        private void ImageBrush_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ((ImageBrush)sender).ImageSource = new BitmapImage(new Uri(@"ms-appx:///Assets/speaker_placeholder.png"));
        }
    }
}
