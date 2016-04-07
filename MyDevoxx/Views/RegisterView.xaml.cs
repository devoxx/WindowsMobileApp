using MyDevoxx.UserControls;
using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkID=390556 dokumentiert.

namespace MyDevoxx.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class RegisterView : Page
    {
        private CameraCaptureControl scanner;
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

        private string confId = default(string);
        private static string USERID = "userId";

        public RegisterView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Wird aufgerufen, wenn diese Seite in einem Frame angezeigt werden soll.
        /// </summary>
        /// <param name="e">Ereignisdaten, die beschreiben, wie diese Seite erreicht wurde.
        /// Dieser Parameter wird normalerweise zum Konfigurieren der Seite verwendet.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            confId = (string)settings.Values["conferenceId"];

            if (settings.Values[USERID + confId] != null)
            { 
                txtUserId.Text = (string)settings.Values[USERID + confId];
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            try
            {
                settings.Values[USERID + confId] = txtUserId.Text;
                if (scanner != null)
                {
                    ContentArea.Children.Remove(scanner);
                    scanner.Unload();
                    scanner = null;
                }
            }
            catch (Exception)
            { }
        }

        void cameraCaptureControlUC_EmailDecoded(object sender, CameraClickedEventArgs e)
        {
            if (e.EncodedData != null)
            {
                string userId = e.EncodedData;
                int endIdx = userId.IndexOf(",");
                if (endIdx > 0)
                {
                    userId = userId.Substring(0, endIdx);
                }
                txtUserId.Text = userId;
                try
                {
                    ContentArea.Children.Remove(scanner);
                    scanner.Unload();
                    scanner = null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[SCANNER]: " + ex.Message);
                }
            }
        }

        private void ActivateQR_Click(object sender, RoutedEventArgs e)
        {
            if (scanner != null)
            {
                return;
            }
            scanner = new CameraCaptureControl();
            scanner.Height = 300;
            scanner.Width = 300;
            ContentArea.Children.Add(scanner);
            this.scanner.UserIdDecoded += cameraCaptureControlUC_EmailDecoded;
            scanner.Initialze();
        }

        private void BackImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }
    }
}
