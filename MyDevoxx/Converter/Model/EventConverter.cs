using MyDevoxx.Model;
using MyDevoxx.Services.RestModel;
using System.Collections.Generic;

namespace MyDevoxx.Converter.Model
{
    public class EventConverter
    {
        public static List<Event> apply(Schedule schedule, string confId)
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
                    }
                    if (s.talk.speakers.Count > 0)
                    {
                        e.speakerNames = e.speakerNames.Remove(e.speakerNames.Length - 2);
                        e.speakerId = e.speakerId.Remove(e.speakerId.Length - 1);
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
                e.fromTime = s.fromTime;
                e.fromTimeMillis = s.fromTimeMillis;
                e.toTime = s.toTime;
                e.toTimeMillis = s.toTimeMillis;
                e.fullTime = e.fromTime + " - " + e.toTime;

                events.Add(e);
            }

            return events;
        }

    }
}
