using MyDevoxx.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyDevoxx.Views
{
    class ScheduleListTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TalkTemplate { get; set; }
        public DataTemplate BreakTemplate { get; set; }
        public DataTemplate ListTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item.GetType().Equals(typeof(Event)))
            {
                Event e = (Event)item;
                if (e.type.Equals(EventType.TALK))
                {
                    return TalkTemplate;
                }
                else if (e.type.Equals(EventType.BREAK))
                {
                    return BreakTemplate;
                }
                else
                {
                    return null;
                }
            }
            else if (item.GetType().Equals(typeof(List<Event>)))
            {
                return ListTemplate;
            }
            else
            {
                return null;
            }
        }
    }
}
