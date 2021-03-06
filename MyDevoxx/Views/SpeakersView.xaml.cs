﻿using MyDevoxx.ViewModel;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
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
    public sealed partial class SpeakersView : Page
    {
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        private static string SEARCH_SETTINGS = "SearchStringSpeakerView";

        public SpeakersView()
        {
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
        }

        /// <summary>
        /// Wird aufgerufen, wenn diese Seite in einem Frame angezeigt werden soll.
        /// </summary>
        /// <param name="e">Ereignisdaten, die beschreiben, wie diese Seite erreicht wurde.
        /// Dieser Parameter wird normalerweise zum Konfigurieren der Seite verwendet.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ((SpeakersViewModel)DataContext).LoadData();

            var searchSettings = settings.Values[SEARCH_SETTINGS];
            if (searchSettings != null && (string)searchSettings != "")
            {
                SpeakerHeader.SearchString = (string)searchSettings;
                SpeakerHeader.ShowSearch();
            }
        }

        private void ImageBrush_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ((ImageBrush)sender).ImageSource = new BitmapImage(new Uri(@"ms-appx:///Assets/speaker_placeholder.png"));
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!typeof(Grid).Equals(sender.GetType()))
            {
                return;
            }
            var nav = ServiceLocator.Current.GetInstance<INavigationService>();
            nav.NavigateTo(ViewModelLocator.SpeakerDetailsViewKey, ((Grid)sender).DataContext);
        }
    }
}
