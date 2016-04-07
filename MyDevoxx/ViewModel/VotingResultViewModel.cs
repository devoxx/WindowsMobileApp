using MyDevoxx.Services;
using MyDevoxx.Services.RestModel.Voting;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDevoxx.ViewModel
{
    public class VotingResultViewModel : ViewModelBase
    {
        private IRestService Service;

        private bool isFetching = false;
        private static string day = "";
        private static string talkType = "";
        private static string track = "";
        private int limit = 100;

        private VoteResults _voteResults;
        public VoteResults VoteResults
        {
            get
            {
                return _voteResults;
            }
            set
            {
                if (Set("VoteResults", ref _voteResults, value))
                {
                    RaisePropertyChanged(() => VoteResults);
                }
            }
        }

        public VotingResultViewModel()
        {
            Service = ServiceLocator.Current.GetInstance<IRestService>();
        }

        public async void LoadData()
        {
            VoteResults = await Service.GetVoteResults(limit, day.ToLower(), talkType, track);
        }
    }
}
