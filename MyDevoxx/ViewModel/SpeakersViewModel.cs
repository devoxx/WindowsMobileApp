using MyDevoxx.Services;
using MyDevoxx.Model;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using Windows.Storage;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using System;
using GalaSoft.MvvmLight.Messaging;
using MyDevoxx.Utils;

namespace MyDevoxx.ViewModel
{
    public class SpeakersViewModel : ViewModelBase
    {
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        private static string SEARCH_SETTINGS = "SearchStringSpeakerView";

        private IDataService Service;
        private bool isLoaded = false;
        private string SearchString = default(string);

        public ObservableCollection<SpeakerGroup> _speakerList = new ObservableCollection<SpeakerGroup>();
        public ObservableCollection<SpeakerGroup> SpeakerList
        {
            get { return _speakerList; }
            set { _speakerList = value; }
        }

        private RelayCommand<string> _searchCommand;
        public RelayCommand<string> SearchCommand
        {
            get { return _searchCommand; }
            private set { _searchCommand = value; }
        }

        public SpeakersViewModel()
        {
            Service = ServiceLocator.Current.GetInstance<IDataService>();
            SearchCommand = new RelayCommand<string>(Search);

            var searchSettings = settings.Values[SEARCH_SETTINGS];
            if (searchSettings != null)
            {
                SearchString = (string)searchSettings;
            }
        }

        public async void LoadData()
        {
            if (isLoaded)
            {
                return;
            }
            isLoaded = true;

            List<Speaker> result;
            if (String.IsNullOrEmpty(SearchString))
            {
                result = await Service.GetSpeakers();
            }
            else
            {
                result = await Service.GetSpeakersBySearchCriteria(SearchString);
            }

            IEnumerable<SpeakerGroup> groups = from item in result
                                               group item by item.firstName.Substring(0, 1).ToUpper() into speakerGroup
                                               select new SpeakerGroup(speakerGroup)
                                               {
                                                   Letter = speakerGroup.Key
                                               };

            foreach (SpeakerGroup s in groups)
            {
                SpeakerList.Add(s);
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            SpeakerList.Clear();
            isLoaded = false;
        }

        private void Search(string searchString)
        {
            SearchString = searchString;
            SpeakerList.Clear();
            isLoaded = false;

            if (String.IsNullOrEmpty(searchString))
            {
                settings.Values[SEARCH_SETTINGS] = null;
            }
            else
            {
                settings.Values[SEARCH_SETTINGS] = searchString;
            }

            LoadData();
            Messenger.Default.Send<MessageType>(MessageType.SEARCH_COMPLETED);
        }
    }

    public class SpeakerGroup : ObservableCollection<Speaker>
    {
        public SpeakerGroup(IEnumerable<Speaker> items) : base(items)
        {
        }

        public string Letter { get; set; }
    }
}
