using MyDevoxx.Model;
using MyDevoxx.Services;
using MyDevoxx.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyDevoxx.ViewModel
{
    public class TracksViewModel : ViewModelBase
    {
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        private static string TRACK_FILTER_SETTINGS = "TrackFilterTrackView";
        private static string DAY_FILTER_SETTINGS = "DayFilterTrackView";

        private string[] DayNames = { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };
        private string[] DayPivotNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

        private IDataService Service;
        private bool isLoaded = false;

        public ObservableCollection<TrackPivotItem> _trackList = new ObservableCollection<TrackPivotItem>();
        public ObservableCollection<TrackPivotItem> TrackList
        {
            get { return _trackList; }
            set { _trackList = value; }
        }

        // Lists for the filter overlay
        private ObservableCollection<FilterItem> _availableTrackFilter = new ObservableCollection<FilterItem>();
        public ObservableCollection<FilterItem> AvailableTrackFilter
        {
            get { return _availableTrackFilter; }
            set { _availableTrackFilter = value; }
        }

        private ObservableCollection<FilterItem> _availableDayFilter = new ObservableCollection<FilterItem>();
        public ObservableCollection<FilterItem> AvailableDayFilter
        {
            get { return _availableDayFilter; }
            set { _availableDayFilter = value; }
        }

        // Lists for the filter action
        private List<String> _choosenTrackFilter = new List<string>();
        public List<String> ChoosenTrackFilter
        {
            get { return _choosenTrackFilter; }
        }

        private List<String> _choosenDayFilter = new List<string>();
        public List<String> ChoosenDayFilter
        {
            get { return _choosenDayFilter; }
        }

        // UI Commands
        private RelayCommand<FilterItem> _trackFilterCommand;
        public RelayCommand<FilterItem> TrackFilterCommand
        {
            get { return _trackFilterCommand; }
            private set { _trackFilterCommand = value; }
        }

        private RelayCommand<FilterItem> _dayFilterCommand;
        public RelayCommand<FilterItem> DayFilterCommand
        {
            get { return _dayFilterCommand; }
            private set { _dayFilterCommand = value; }
        }

        public TracksViewModel()
        {
            Service = ServiceLocator.Current.GetInstance<IDataService>();
            TrackFilterCommand = new RelayCommand<FilterItem>(TrackFilterTapped);
            DayFilterCommand = new RelayCommand<FilterItem>(DayFilterTapped);
            Messenger.Default.Register<MessageType>(this, ApplyFilter);
            Messenger.Default.Register<MessageType>(this, ClearFilter);

            var trackFilterSettings = settings.Values[TRACK_FILTER_SETTINGS];
            if (trackFilterSettings != null)
            {
                foreach (string s in (string[])trackFilterSettings)
                {
                    ChoosenTrackFilter.Add(s);
                }
            }

            var dayFilterSettings = settings.Values[DAY_FILTER_SETTINGS];
            if (dayFilterSettings != null)
            {
                foreach (string s in (string[])dayFilterSettings)
                {
                    ChoosenDayFilter.Add(s);
                }
            }
        }

        public async void LoadData()
        {
            if (isLoaded)
            {
                return;
            }
            isLoaded = true;

            List<Track> result = await Service.GetTracks();
            foreach (Track t in from r in result orderby r.title select r)
            {
                if (ChoosenTrackFilter.Contains(t.id))
                {
                    continue;
                }

                List<Event> events = await Service.GetEventsByTrack(t.id);
                TrackList.Add(new TrackPivotItem(t, events.Where(e => !ChoosenDayFilter.Contains(e.day)).OrderByDescending(e => e.Starred).ToList()));
            }

            if (AvailableTrackFilter.Count == 0)
            {
                foreach (Track t in from r in result orderby r.title select r)
                {
                    AvailableTrackFilter.Add(new FilterItem(t.title, t.id, !ChoosenTrackFilter.Contains(t.id)));
                }
            }
            fillAvailableDayFilters();
        }

        private async void fillAvailableDayFilters()
        {
            Conference conference = await Service.GetCurrentConference();

            DateTime fromDate, toDate;
            bool isSuccessfulFrom = DateTime.TryParse(conference.fromDate, out fromDate);
            bool isSuccessfulTo = DateTime.TryParse(conference.toDate, out toDate);
            int dayCnt = (int)(toDate - fromDate).TotalDays;
            int dayStart = (int)fromDate.DayOfWeek;

            if (AvailableDayFilter.Count == 0)
            {
                for (int i = 0; i <= dayCnt; i++)
                {
                    int day = fromDate.AddDays(i).Day;
                    string title = DayPivotNames[i + dayStart] + " " + day;
                    string key = DayNames[i + dayStart];
                    AvailableDayFilter.Add(new FilterItem(title, key, !ChoosenDayFilter.Contains(key)));
                }
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            TrackList.Clear();
            isLoaded = false;

            ChoosenTrackFilter.Clear();
            AvailableTrackFilter.Clear();
            ChoosenDayFilter.Clear();
            AvailableDayFilter.Clear();
            settings.Values[TRACK_FILTER_SETTINGS] = null;
            settings.Values[DAY_FILTER_SETTINGS] = null;
        }

        private void ApplyFilter(MessageType type)
        {
            if (type.Equals(MessageType.REFRESH_TRACKS))
            {
                isLoaded = false;
                TrackList.Clear();

                if (ChoosenTrackFilter.Count > 0)
                {
                    settings.Values[TRACK_FILTER_SETTINGS] = ChoosenTrackFilter.ToArray();
                }
                else
                {
                    settings.Values[TRACK_FILTER_SETTINGS] = null;
                }
                if (ChoosenDayFilter.Count > 0)
                {
                    settings.Values[DAY_FILTER_SETTINGS] = ChoosenDayFilter.ToArray();
                }
                else
                {
                    settings.Values[DAY_FILTER_SETTINGS] = null;
                }

                LoadData();
            }
        }

        private void ClearFilter(MessageType type)
        {
            if (type.Equals(MessageType.CLEAR_FILTER_TRACKS))
            {
                isLoaded = false;
                TrackList.Clear();
                ChoosenTrackFilter.Clear();
                AvailableTrackFilter.Clear();
                ChoosenDayFilter.Clear();
                AvailableDayFilter.Clear();
                settings.Values[TRACK_FILTER_SETTINGS] = null;
                settings.Values[DAY_FILTER_SETTINGS] = null;

                LoadData();
            }
        }

        public void TrackFilterTapped(FilterItem item)
        {
            if (ChoosenTrackFilter.Contains(item.FilterValue))
            {
                ChoosenTrackFilter.Remove(item.FilterValue);
                item.IsChecked = true;
            }
            else
            {
                ChoosenTrackFilter.Add(item.FilterValue);
                item.IsChecked = false;
            }
        }

        public void DayFilterTapped(FilterItem item)
        {
            if (ChoosenDayFilter.Contains(item.FilterValue))
            {
                ChoosenDayFilter.Remove(item.FilterValue);
                item.IsChecked = true;
            }
            else
            {
                ChoosenDayFilter.Add(item.FilterValue);
                item.IsChecked = false;
            }
        }
    }

    public class TrackPivotItem
    {
        public Track track { get; set; }
        public List<Event> events { get; set; }

        public TrackPivotItem(Track track, List<Event> events)
        {
            this.track = track;
            this.events = events;
        }
    }
}
