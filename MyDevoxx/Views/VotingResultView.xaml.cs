using MyDevoxx.Services;
using MyDevoxx.Services.RestModel.Voting;
using MyDevoxx.ViewModel;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyDevoxx.Views
{
    public sealed partial class VotingResultView : Page
    {
        private VotingResultViewModel vm;

        public VotingResultView()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            vm = ((VotingResultViewModel)DataContext);

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
        }

        private void Header_FilterTapped(object sender)
        {

        }

        private void TrackFilterGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void DayFilterGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void Apply_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void Clear_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
