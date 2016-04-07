using SQLite.Net.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyDevoxx.Model
{
    public class Vote : INotifyPropertyChanged
    {
        [PrimaryKey]
        public string id { get; set; }
        public string talkId { get; set; }
        public string confId { get; set; }
        public int _rating = default(int);
        public int rating
        {
            get { return _rating; }
            set
            {
                if(IsSent)
                {
                    return;
                }
                _rating = value;
                NotifyPropertyChanged();
            }
        }
        public string _content = default(string);
        public string content
        {
            get { return _content; }
            set
            {
                _content = value;
                NotifyPropertyChanged();
            }
        }
        public string _delivery = default(string);
        public string delivery
        {
            get { return _delivery; }
            set
            {
                _delivery = value;
                NotifyPropertyChanged();
            }
        }
        public string _other = default(string);
        public string other
        {
            get { return _other; }
            set
            {
                _other = value;
                NotifyPropertyChanged();
            }
        }
        public bool _isSent = false;
        public bool IsSent
        {
            get { return _isSent; }
            set
            {
                _isSent = value;
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
