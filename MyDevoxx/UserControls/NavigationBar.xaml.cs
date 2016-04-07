using MyDevoxx.ViewModel;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// Die Elementvorlage "Benutzersteuerelement" ist unter http://go.microsoft.com/fwlink/?LinkId=234236 dokumentiert.

namespace MyDevoxx.UserControls
{
    public sealed partial class NavigationBar : UserControl
    {
        public enum Buttons
        {
            SCHEDULE,
            TRACKS,
            SPEAKERS,
            MAP,
            UNKNOWN
        }

        public static readonly DependencyProperty CurrentProperty = 
            DependencyProperty.Register("Current", typeof(Buttons), typeof(NavigationBar), new PropertyMetadata(Buttons.UNKNOWN));

        public Buttons Current
        {
            get { return (Buttons)GetValue(CurrentProperty); }
            set { SetValue(CurrentProperty, value); }
        }

        public NavigationBar()
        {
            this.InitializeComponent();
        }


        private void Speakers_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var nav = ServiceLocator.Current.GetInstance<INavigationService>();
            if (!nav.CurrentPageKey.Equals(ViewModelLocator.SpeakersViewKey))
            {
                nav.NavigateTo(ViewModelLocator.SpeakersViewKey);
            }
        }

        private void Schedule_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var nav = ServiceLocator.Current.GetInstance<INavigationService>();
            if (!nav.CurrentPageKey.Equals(ViewModelLocator.ScheduleViewKey))
            {
                nav.NavigateTo(ViewModelLocator.ScheduleViewKey);
            }
        }

        private void Tracks_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var nav = ServiceLocator.Current.GetInstance<INavigationService>();
            if (!nav.CurrentPageKey.Equals(ViewModelLocator.TracksViewKey))
            {
                nav.NavigateTo(ViewModelLocator.TracksViewKey);
            }
        }

        private void Map_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var nav = ServiceLocator.Current.GetInstance<INavigationService>();
            if (!nav.CurrentPageKey.Equals(ViewModelLocator.MapViewKey))
            {
                nav.NavigateTo(ViewModelLocator.MapViewKey);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            switch (Current)
            {
                case Buttons.SCHEDULE:
                    {
                        ScheduleImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Menu/schedule.png"));
                        Schdule.Foreground = new SolidColorBrush(Colors.Black);
                        break;
                    }
                case Buttons.TRACKS:
                    {
                        TracksImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Menu/tracks.png"));
                        Tracks.Foreground = new SolidColorBrush(Colors.Black);
                        break;
                    }
                case Buttons.SPEAKERS:
                    {
                        SpeakersImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Menu/speaker.png"));
                        Speakers.Foreground = new SolidColorBrush(Colors.Black);
                        break;
                    }
                case Buttons.MAP:
                    {
                        MapImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Menu/map.png"));
                        Map.Foreground = new SolidColorBrush(Colors.Black);
                        break;
                    }
            }
        }
    }
}
