using MyDevoxx.UserControls;
using MyDevoxx.ViewModel;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkID=390556 dokumentiert.

namespace MyDevoxx.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class SpeakerDetailsView : Page
    {
        public SpeakerDetailsView()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ((SpeakerDetailsViewModel)DataContext).LoadData(e.Parameter);
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private void SpeakerImageBrush_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ((ImageBrush)sender).ImageSource = new BitmapImage(new Uri(@"ms-appx:///Assets/FallbackData/people.png"));
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
    }
}
