using MyDevoxx.Model;
using MyDevoxx.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace MyDevoxx.UserControls
{
    public sealed partial class ConferenceSelector : UserControl, INotifyPropertyChanged
    {
        public event EventHandler<Conference> ConferenceSelected;

        public enum Quadrants : int { nw = 2, ne = 1, sw = 4, se = 3 }
        private double startedAngle;
        private double diffAngle;
        private double _previousAngle;
        private bool isGlobeRotating = false;

        private double _Angle = default(double);
        public double Angle { get { return _Angle; } set { SetProperty(ref _Angle, value); } }

        public readonly DependencyProperty ItemListProperty =
            DependencyProperty.Register("ItemList", typeof(ObservableCollection<SelectorItem>), typeof(ConferenceSelector),
                new PropertyMetadata(new ObservableCollection<SelectorItem>()));

        public ObservableCollection<SelectorItem> ItemList
        {
            get
            {
                return (ObservableCollection<SelectorItem>)GetValue(ItemListProperty);
            }
            set
            {
                SetValue(ItemListProperty, value);
            }
        }

        public ConferenceSelector()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }

        public void setConference(string id)
        {
            foreach (SelectorItem i in ItemList)
            {
                if (i.Conference.id.Equals(id))
                {
                    this.Angle = 360 - i.IconAngle;
                    return;
                }
            }
        }

        public void setConference(int i)
        {
            this.Angle = 360 - ItemList[i].IconAngle;
        }

        private double GetAngle(Point touchPoint, Size circleSize)
        {
            var _X = touchPoint.X - (circleSize.Width / 2d);
            var _Y = circleSize.Height - touchPoint.Y - (circleSize.Height / 2d);
            var _Hypot = Math.Sqrt(_X * _X + _Y * _Y);
            var _Value = Math.Asin(_Y / _Hypot) * 180 / Math.PI;
            var _Quadrant = (_X >= 0) ?
                (_Y >= 0) ? Quadrants.ne : Quadrants.se :
                (_Y >= 0) ? Quadrants.nw : Quadrants.sw;
            switch (_Quadrant)
            {
                case Quadrants.ne: _Value = 090 - _Value; break;
                case Quadrants.nw: _Value = 270 + _Value; break;
                case Quadrants.se: _Value = 090 - _Value; break;
                case Quadrants.sw: _Value = 270 + _Value; break;
            }
            var resultangle = _Value - diffAngle;
            return resultangle;
        }

        private double GetAngleForStartingPoint(Point touchPoint, Size circleSize)
        {
            var _X = touchPoint.X - (circleSize.Width / 2d);
            var _Y = circleSize.Height - touchPoint.Y - (circleSize.Height / 2d);
            var _Hypot = Math.Sqrt(_X * _X + _Y * _Y);
            var _Value = Math.Asin(_Y / _Hypot) * 180 / Math.PI;
            var _Quadrant = (_X >= 0) ?
                (_Y >= 0) ? Quadrants.ne : Quadrants.se :
                (_Y >= 0) ? Quadrants.nw : Quadrants.sw;
            switch (_Quadrant)
            {
                case Quadrants.ne: _Value = 090 - _Value; break;
                case Quadrants.nw: _Value = 270 + _Value; break;
                case Quadrants.se: _Value = 090 - _Value; break;
                case Quadrants.sw: _Value = 270 + _Value; break;
            }
            return _Value;
        }

        private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (isGlobeRotating)
            {
                return;
            }

            _previousAngle = Angle;
            this.Angle = GetAngle(e.Position, this.RenderSize);
            //var resultangle = Angle;
            //if (resultangle < 0.00)
            //{
            //    resultangle = Math.Abs(resultangle);
            //}
            //if (resultangle > 360.00)
            //{
            //    resultangle = resultangle % 360.00;
            //}
        }

        private void Grid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (isGlobeRotating)
            {
                return;
            }

            storyBoard.Stop();

            startedAngle = Angle;
            double diffangle = GetAngleForStartingPoint(e.Position, this.RenderSize);
            diffAngle = diffangle - Angle;
        }

        private void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (isGlobeRotating)
            {
                return;
            }

            if (ItemList.Count == 0)
            {
                return;
            }

            double range = 360 / ItemList.Count;
            double quadrant = Math.Round(Angle / range);
            double finalAngle = quadrant * range;

            doubleAnimation.From = Angle;
            doubleAnimation.To = finalAngle;
            storyBoard.Begin();

            Angle = finalAngle;

            quadrant = quadrant * -1;
            if (quadrant < 0)
            {
                while (quadrant < 0)
                {
                    quadrant = quadrant + ItemList.Count;
                }
            }
            else {
                while (quadrant >= ItemList.Count)
                {
                    quadrant = quadrant - ItemList.Count;
                }
            }

            if (ItemList.Count > (int)quadrant)
            {
                SelectorItem i = ItemList[(int)quadrant];
                if (ConferenceSelected != null)
                {
                    ConferenceSelected(this, i.Conference);
                }
            }
        }

        public void RotateGlobe(bool rotate)
        {
            if (rotate)
            {
                isGlobeRotating = true;
                LoadingStoryBoard.Begin();
            }
            else
            {
                isGlobeRotating = false;
                LoadingStoryBoard.Stop();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (!object.Equals(storage, value))
            {
                storage = value;
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async void Image_ImageFailed(object sender, Windows.UI.Xaml.ExceptionRoutedEventArgs e)
        {
            if (typeof(Image).Equals(sender.GetType()))
            {
                Image i = (Image)sender;
                BitmapImage bi = (BitmapImage)i.Source;
                string imageName = bi.UriSource.Segments[bi.UriSource.Segments.Length - 1];
                if (await FileUtils.FileExists(@"ms-appx:///Assets/FallbackData/" + imageName))
                {
                    ((Image)sender).Source = new BitmapImage(new System.Uri(@"ms-appx:///Assets/FallbackData/" + imageName));
                }
            }
        }

        private void LoadingStoryBoard_Completed(object sender, object e)
        {
            isGlobeRotating = false;
        }

        public void cleanUp()
        {
            ItemList.Clear();
            ItemListControl.Items.Clear();
        }
    }

    public class SelectorItem
    {
        public double IconAngle { get; set; }
        public string IconPath { get; set; }
        public Conference Conference { get; set; }
        public int IconHeight { get; set; }

        public SelectorItem(Conference conference, double angle)
        {
            Conference = conference;
            IconPath = conference.confIcon;
            IconAngle = angle;
            IconHeight = 100;
        }
    }
}
