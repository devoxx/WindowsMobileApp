using System;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyDevoxx.UserControls
{
    public sealed partial class EventTalkControl : UserControl
    {
        public EventTalkControl()
        {
            this.InitializeComponent();
        }

        private void ImageBrush_ImageFailed(object sender, Windows.UI.Xaml.ExceptionRoutedEventArgs e)
        {
            ((ImageBrush)sender).ImageSource = new BitmapImage(new Uri(@"ms-appx:///Assets/speaker_placeholder.png"));
        }
    }
}
