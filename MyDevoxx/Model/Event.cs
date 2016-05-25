using SQLite.Net.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyDevoxx.Model
{
    public class Event : INotifyPropertyChanged
    {
        // Primarykey/dbId is the confId + id
        [PrimaryKey]
        public string dbId { get; set; }
        public string confId { get; set; }
        // id which is defined in the REST data
        public string id { get; set; }
        public string day { get; set; }
        public EventType type { get; set; }

        public string roomName { get; set; }
        public string roomId { get; set; }

        public string fromTime { get; set; }
        public long fromTimeMillis { get; set; }
        public string toTime { get; set; }
        public long toTimeMillis { get; set; }
        public string fullTime { get; set; }

        public string title { get; set; }
        public string summary { get; set; }
        public string lang { get; set; }

        public string trackId { get; set; }
        public string trackName { get; set; }
        public string trackImage { get; set; }

        public string speakerImage { get; set; }

        public string talkType { get; set; }

        // comma seperated list of speaker names
        public string speakerNames { get; set; }
        // comma seperated list of speaker ids
        public string speakerId { get; set; }

        private bool _starred = false;

        public bool Starred
        {
            get
            {
                return _starred;
            }
            set
            {
                _starred = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
