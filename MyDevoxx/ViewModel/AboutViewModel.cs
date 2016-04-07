using System;
using MyDevoxx.Model;
using MyDevoxx.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using Windows.System;

namespace MyDevoxx.ViewModel
{
    public class AboutViewModel : ViewModelBase
    {
        private IDataService DataService;
        private ITwitterService TwitterService;

        public RelayCommand _twitterCommand;
        public RelayCommand TwitterCommand
        {
            get { return _twitterCommand; }
            set { _twitterCommand = value; }
        }

        public RelayCommand _websiteCommand;
        public RelayCommand WebsiteCommand
        {
            get { return _websiteCommand; }
            set { _websiteCommand = value; }
        }

        private Conference _conference;
        public Conference Conference
        {
            get { return _conference; }
            set
            {
                if (Set("Conference", ref _conference, value))
                {
                    RaisePropertyChanged(() => Conference);
                }
            }
        }

        public AboutViewModel()
        {
            DataService = ServiceLocator.Current.GetInstance<IDataService>();
            TwitterService = ServiceLocator.Current.GetInstance<ITwitterService>();
            TwitterCommand = new RelayCommand(OnTwitter_Tapped);
            WebsiteCommand = new RelayCommand(OnWebsite_Tapped);
        }

        public async void onLoad()
        {
            Conference = await DataService.GetCurrentConference();
        }

        private void OnTwitter_Tapped()
        {
            TwitterService.goToHashTag(Conference.hashtag);
        }
        
        private async void OnWebsite_Tapped()
        {
            await Launcher.LaunchUriAsync(new Uri(Conference.wwwURL));
        }
    }
}
