using SQLite.Net.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyDevoxx.Model
{
    public class Note : INotifyPropertyChanged
    {
        [PrimaryKey]
        public string id { get; set; }
        public string confId { get; set; }
        public string talkId { get; set; }

        private string _note = default(string);
        public string note
        {
            get { return _note; }
            set
            {
                _note = value;
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
