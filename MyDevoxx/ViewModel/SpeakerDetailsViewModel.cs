using MyDevoxx.Model;
using MyDevoxx.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.ObjectModel;
using Windows.System;

namespace MyDevoxx.ViewModel
{
    public class SpeakerDetailsViewModel : ViewModelBase
    {
        private IDataService Service;
        private ITwitterService TwitterService;

        private Speaker _speaker;
        public Speaker Speaker
        {
            get
            {
                return _speaker;
            }
            set
            {
                if (Set("Speaker", ref _speaker, value))
                {
                    RaisePropertyChanged(() => Speaker);
                }
            }
        }

        private ObservableCollection<Event> _eventList = new ObservableCollection<Event>();
        public ObservableCollection<Event> EventList
        {
            get
            {
                return _eventList;
            }
        }

        private RelayCommand _blogCommand;
        public RelayCommand BlogCommand
        {
            get
            {
                return _blogCommand;
            }
            private set
            {
                _blogCommand = value;
            }
        }

        private RelayCommand _twitterCommand;
        public RelayCommand TwitterCommand
        {
            get
            {
                return _twitterCommand;
            }
            private set
            {
                _twitterCommand = value;
            }
        }

        public SpeakerDetailsViewModel()
        {
            Service = ServiceLocator.Current.GetInstance<IDataService>();
            TwitterService = ServiceLocator.Current.GetInstance<ITwitterService>();
            BlogCommand = new RelayCommand(Blog_Tapped);
            TwitterCommand = new RelayCommand(Twitter_Tapped);
        }

        public async void LoadData(object param)
        {
            this.EventList.Clear();
            this.Speaker = null;

            string uuid;
            if (param.GetType().Equals(typeof(string)))
            {
                uuid = (string)param;
            } else
            {
                uuid = ((Speaker)param).uuid;
            }
            this.Speaker = await Service.GetSpeaker(uuid);
            if (this.Speaker.talkIds != null && this.Speaker.talkIds.Length > 0)
            {
                string[] talkIds = this.Speaker.talkIds.Split(',');
                foreach (string id in talkIds)
                {
                    Event e = await Service.GetEventsById(id);
                    if (e != null)
                    {
                        this.EventList.Add(e);
                    }
                }
            }
        }

        private void Twitter_Tapped()
        {
            TwitterService.sendSpeakerTweet(this.Speaker);
        }

        private async void Blog_Tapped()
        {
            await Launcher.LaunchUriAsync(new Uri(this.Speaker.checkedBlog));
        }
    }
}
