using MyDevoxx.Model;
using MyDevoxx.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Messaging;
using MyDevoxx.Services.RestModel.Voting;
using System;
using System.Diagnostics;
using GalaSoft.MvvmLight.Views;

namespace MyDevoxx.ViewModel
{
    public class TalkDetailsViewModel : ViewModelBase
    {
        private IDataService DataService;
        private ITwitterService TwitterService;
        private IVotingService VotingService;

        private string _votingImage;
        public string VotingImage
        {
            get { return _votingImage; }
            set
            {
                var image = "ms-appx:///Assets/Voting/" + value + ".png";
                if (Set("VotingImage", ref _votingImage, image))
                {
                    RaisePropertyChanged(() => VotingImage);
                }
            }
        }

        private bool _votingEnabled;
        public bool VotingEnabled
        {
            get { return _votingEnabled; }
            set
            {
                if (Set("VotingEnabled", ref _votingEnabled, value))
                {
                    RaisePropertyChanged(() => VotingEnabled);
                }
            }
        }

        private Event _event;
        public Event Event
        {
            get { return _event; }
            set { _event = value; }
        }

        private Note _note;
        public Note Note
        {
            get { return _note; }
            set
            {
                if (Set("Note", ref _note, value))
                {
                    RaisePropertyChanged(() => Note);
                }
            }
        }

        private Vote _vote;
        public Vote Vote
        {
            get { return _vote; }
            set
            {
                if (Set("Vote", ref _vote, value))
                {
                    RaisePropertyChanged(() => Vote);
                }
            }
        }

        private RelayCommand _starCommand;
        public RelayCommand StarCommand
        {
            get { return _starCommand; }
            private set { _starCommand = value; }
        }

        private RelayCommand _twitterCommand;
        public RelayCommand TwitterCommand
        {
            get { return _twitterCommand; }
            private set { _twitterCommand = value; }
        }

        public TalkDetailsViewModel()
        {
            DataService = ServiceLocator.Current.GetInstance<IDataService>();
            TwitterService = ServiceLocator.Current.GetInstance<ITwitterService>();
            VotingService = ServiceLocator.Current.GetInstance<IVotingService>();

            StarCommand = new RelayCommand(Star_Tapped);
            TwitterCommand = new RelayCommand(Twitter_Tapped);
        }

        public async void LoadData(Event e)
        {
            this.Event = e;
            this.Note = await DataService.GetNote(Event.id);

            Conference conference = await DataService.GetCurrentConference();
            this.VotingImage = conference.votingImageName;

            bool votingFlag = false;
            Boolean.TryParse(conference.votingEnabled, out votingFlag);

            DateTime fromDate;
            bool isSuccessfulParsed = DateTime.TryParse(conference.fromDate, out fromDate);

            // enable voting when cfp enables voting and the todays date is equal or greater than the conference start date
            this.VotingEnabled = votingFlag && isSuccessfulParsed && fromDate.Date <= DateTime.Now.Date;

            this.Vote = await DataService.GetVote(Event.id);
        }

        private async void Star_Tapped()
        {
            Event.Starred = Event.Starred ? false : true;
            await DataService.UpdateEvent(Event);

            Messenger.Default.Send(Event);
        }

        private void Twitter_Tapped()
        {
            TwitterService.sendTalkTweet(this.Event);
        }

        public void SaveNote()
        {
            DataService.SaveOrUpdateNote(this.Note);
        }

        public async void SendVote()
        {
            await DataService.SaveOrUpdateVote(this.Vote);
            if (this.Vote.rating < 1)
            {
                return;
            }
            VoteMessage voteMessage = await VotingService.Vote(this.Vote);
            if (voteMessage != null &&
                (!string.IsNullOrWhiteSpace(voteMessage.message) || !string.IsNullOrWhiteSpace(voteMessage.messaage)))
            {
                IDialogService dialogService = ServiceLocator.Current.GetInstance<IDialogService>();
                await dialogService.ShowMessage(voteMessage.messaage + voteMessage.message, "I'm sorry!");
            }
            else
            {
                this.Vote.IsSent = true;
                await DataService.SaveOrUpdateVote(this.Vote);
            }

        }
    }
}
