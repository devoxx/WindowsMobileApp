using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyDevoxx.ViewModel
{
    public class FilterItem : INotifyPropertyChanged
    {
        private string _title = default(string);
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                NotifyPropertyChanged();
            }
        }
        private bool _isChecked = default(bool);
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                NotifyPropertyChanged();
            }
        }
        private string _filterValue = default(string);
        public string FilterValue
        {
            get
            {
                return _filterValue;
            }
            set
            {
                _filterValue = value;
                NotifyPropertyChanged();
            }
        }

        public FilterItem(string title, string filterValue, bool isChecked)
        {
            this.Title = title;
            this.FilterValue = filterValue;
            this.IsChecked = isChecked;
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
