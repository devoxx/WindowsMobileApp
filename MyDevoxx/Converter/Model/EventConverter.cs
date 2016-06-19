using MyDevoxx.Model;
using MyDevoxx.Services.RestModel;
using System;
using System.Collections.Generic;

namespace MyDevoxx.Converter.Model
{
    public class EventConverter
    {
        private static readonly Dictionary<string, int> timeOffsetDic = new Dictionary<string, int>
        {
            { "UK", 1 },
            { "Morocco", 0 },
            { "Belgium", 1 },
            { "Poland", 2 },
            { "France", 2 }
        };


        public static List<Event> apply(Schedule schedule, string confId, string country)
        {
            List<Event> events = new List<Event>();
            foreach (Slot s in schedule.slots)
            {
                Event e = new Event();
                if (s.talk != null)
                {
                    e.id = s.talk.id;
                    e.title = s.talk.title;
                    e.trackId = s.talk.trackId;
                    e.trackName = s.talk.track;
                    e.talkType = s.talk.talkType;
                    e.summary = s.talk.summary;
                    e.lang = s.talk.lang;

                    e.speakerNames = "";
                    e.speakerId = "";
                    foreach (SpeakerSimple ss in s.talk.speakers)
                    {
                        e.speakerNames += ss.name + ", ";
                        e.speakerId += ss.link.href.Substring(ss.link.href.LastIndexOf("/") + 1) + ",";
                        if (e.speakerImage == null)
                        {
                            e.speakerImage = e.speakerId;
                        }
                    }
                    if (s.talk.speakers.Count > 0)
                    {
                        e.speakerNames = e.speakerNames.Remove(e.speakerNames.Length - 2);
                        e.speakerId = e.speakerId.Remove(e.speakerId.Length - 1);
                        e.speakerImage = e.speakerImage.Remove(e.speakerImage.Length - 1);
                    }
                    e.type = EventType.TALK;
                }
                else if (s.@break != null)
                {
                    e.id = s.@break.id;
                    e.title = s.@break.nameEN;
                    e.type = EventType.BREAK;
                }
                else
                {
                    continue;
                }

                e.dbId = confId + "_" + s.day + "_" + e.id;
                e.confId = confId;
                e.day = s.day;
                e.roomName = s.roomName;
                e.roomId = s.roomId;

                e.fromTimeMillis = s.fromTimeMillis;
                e.toTimeMillis = s.toTimeMillis;

                int timeOffset;
                if(!timeOffsetDic.TryGetValue(country, out timeOffset))
                {
                    timeOffset = 0;
                }

                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var StartDateTime = epoch.AddMilliseconds(s.fromTimeMillis).AddHours(timeOffset);
                var EndDateTime = epoch.AddMilliseconds(s.toTimeMillis).AddHours(timeOffset);

                e.fromTime = StartDateTime.ToString("HH:mm");
                e.toTime = EndDateTime.ToString("HH:mm");
                e.fullTime = e.fromTime + " - " + e.toTime;

                events.Add(e);
            }

            return events;
        }

    }
}
