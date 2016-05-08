using MyDevoxx.Model;
using MyDevoxx.Services;
using MyDevoxx.UserControls;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using System;

namespace MyDevoxx.ViewModel
{
    public class ConferenceSelectorViewModel : ViewModelBase
    {
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        private IDataService Service;

        private bool isLoaded = false;

        public Conference _conference;
        public Conference Conference
        {
            get
            {
                return _conference;
            }
            set
            {
                if (Set("Conference", ref _conference, value))
                {
                    RaisePropertyChanged(() => Conference);
                }
            }
        }

        public ObservableCollection<SelectorItem> _selectorItemList = new ObservableCollection<SelectorItem>();
        public ObservableCollection<SelectorItem> SelectorItemList
        {
            get
            {
                return _selectorItemList;
            }
            set
            {
                _selectorItemList = value;
                RaisePropertyChanged(() => SelectorItemList);
            }
        }

        public ConferenceSelectorViewModel()
        {
            Service = ServiceLocator.Current.GetInstance<IDataService>();
        }

        public async void LoadData()
        {
            if (isLoaded)
            {
                return;
            }
            isLoaded = true;

            List<Conference> conferences = await Service.FetchConferences();
            conferences.Sort((a, b) =>
            {
                DateTime aDate, bDate;
                if (!DateTime.TryParse(a.toDate, out aDate)) return 1;
                if (!DateTime.TryParse(b.toDate, out bDate)) return -1;
                int aComp = aDate.CompareTo(DateTime.Today);
                int bComp = bDate.CompareTo(DateTime.Today);
                int cComp = aDate.CompareTo(bDate);
                if (aComp == bComp) return cComp;
                if (aComp > bComp) return bComp;
                return aComp;
            });

            int cnt = conferences.Count;
            int i = 0;
            foreach (Conference conference in conferences)
            {
                int angle = (360 / cnt) * i++;
                SelectorItemList.Add(new SelectorItem(conference, angle));
            }
            if (conferences.Count > 0)
            {
                Conference = conferences[0];
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            Conference = null;
            SelectorItemList.Clear();
            isLoaded = false;
        }
    }
}
