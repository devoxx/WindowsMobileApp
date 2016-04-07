using MyDevoxx.Model;
using MyDevoxx.Services;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace MyDevoxx.ViewModel
{
    public class MapViewModel : ViewModelBase
    {
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        private IDataService Service;
        public bool isLoaded = false;

        private Conference _conference;
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
        public Geopoint ConferenceLocation { get; set; }
        private ObservableCollection<object> _mapList = new ObservableCollection<object>();
        public ObservableCollection<object> MapList
        {
            get
            { return _mapList; }
            set
            { _mapList = value; }
        }

        public MapViewModel()
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

            MapList.Clear();
            List<Model.Floor> result = await Service.GetFloors("phone");
            foreach (Model.Floor f in result)
            {
                MapList.Add(f);
            }

            Conference = await Service.GetCurrentConference();
            ConferenceLocation = new Geopoint(new BasicGeoposition()
            {
                Latitude = Double.Parse(Conference.latitude),
                Longitude = Double.Parse(Conference.longitude)
            });
            MapList.Add(ConferenceLocation);
        }

        public override void Cleanup()
        {
            isLoaded = false;
        }
    }
}
