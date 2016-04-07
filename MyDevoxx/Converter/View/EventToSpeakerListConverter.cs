using MyDevoxx.Model;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace MyDevoxx.Converter.View
{
    public class EventToSpeakerListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return new List<Speaker>();
            }

            Event e = (Event)value;
            string[] speakerNames = e.speakerNames.Split(',');
            string[] speakerIds = e.speakerId.Split(',');
            List<Speaker> result = new List<Speaker>();
            for (int i = 0; i < speakerNames.Length; i++)
            {
                result.Add(new Speaker()
                {
                    firstName = speakerNames[i].Trim(),
                    uuid = speakerIds[i]
                });
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
