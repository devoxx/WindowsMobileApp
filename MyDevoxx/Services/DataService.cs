using MyDevoxx.Model;
using Microsoft.Practices.ServiceLocation;
using SQLite.Net;
using SQLite.Net.Async;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyDevoxx.Services
{
    public class DataService : IDataService
    {
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        private SQLiteAsyncConnection sqlConnection;
        private IRestService Service;

        private string[] DayNames = { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday" };

        public DataService()
        {
            DBConnection();
            Service = ServiceLocator.Current.GetInstance<IRestService>();
        }

        public async Task<List<Conference>> FetchConferences()
        {
            List<Conference> conferences = await Service.GetConferences();
            await sqlConnection.InsertOrReplaceAllAsync(conferences);

            return conferences;
        }

        public async void CollectAll(FinishedLoading callback)
        {
            try
            {
                if (!await CollectEvents())
                {
                    callback(false);
                    return;
                }
                await CollectTracks();
                await CollectSpeakers();
                await CollectFloors();
                callback(true);
            }
            catch
            {
                callback(false);
            }
        }

        public async void UpdateAll(FinishedLoading callback)
        {
            try
            {
                await FetchConferences();
                Conference conference = await GetCurrentConference();
                settings.Values["conferenceId"] = conference.id;
                settings.Values["cfpEndpoint"] = conference.cfpEndpoint;
                settings.Values["talkURL"] = conference.talkURL;
                settings.Values["country"] = conference.country;
                settings.Values["votingURL"] = conference.votingURL;

                if (!await CollectEvents())
                {
                    callback(false);
                    return;
                }
                await CollectTracks();
                await CollectSpeakers();
                await CollectFloors();
                callback(true);
            }
            catch
            {
                callback(false);
            }
        }

        public async Task<bool> CollectEvents()
        {
            List<Track> tracks = await Service.GetTracks();
            var trackDic = tracks.Select(track => new { Key = track.id, Value = track.imgsrc })
                .Distinct()
                .ToDictionary(track => track.Key, track => track.Value, StringComparer.OrdinalIgnoreCase);

            string confId = currentConferenceId();
            Conference conference = await sqlConnection.Table<Conference>().Where(c => c.id.Equals(confId)).FirstOrDefaultAsync();

            DateTime fromDate, toDate;
            bool isSuccessfulFrom = DateTime.TryParse(conference.fromDate, out fromDate);
            bool isSuccessfulTo = DateTime.TryParse(conference.toDate, out toDate);
            int dayCnt = (int)(toDate - fromDate).TotalDays;
            int dayStart = (int)fromDate.DayOfWeek;

            List<Event> starredEvents = await GetStarredEvents();
            Dictionary<string, Event> starredEventsDic = starredEvents.ToDictionary(p => p.id, p => p);

            // delete all events
            await ClearEvents(confId);

            string cfpUrl = conference.cfpURL;
            int lastIdx = conference.cfpURL.LastIndexOf('/');
            if (lastIdx + 1 == conference.cfpURL.Length)
            {
                cfpUrl = conference.cfpURL.Substring(0, conference.cfpURL.Length - 1);
            }

            bool isScheduleAvailable = false;
            for (int i = dayStart; i <= dayStart + dayCnt; i++)
            {
                List<Event> events = await Service.GetEvents(DayNames[i]);
                foreach (Event e in events)
                {
                    // if there are no talks, then conference is not ready yet
                    if (!isScheduleAvailable && EventType.TALK.Equals(e.type))
                    {
                        isScheduleAvailable = true;
                    }

                    if (e.trackId != null)
                    {
                        e.trackImage = cfpUrl + trackDic[e.trackId];
                    }
                    if (starredEventsDic.ContainsKey(e.id))
                    {
                        e.Starred = true;
                    }
                }
                await sqlConnection.InsertOrReplaceAllAsync(events);
            }
            return isScheduleAvailable;
        }

        public async Task CollectTracks()
        {
            List<Track> tracks = await Service.GetTracks();
            await ClearTracks(currentConferenceId());
            await sqlConnection.InsertOrReplaceAllAsync(tracks);
        }

        public async Task CollectFloors()
        {
            List<Model.Floor> floors = await Service.GetFloors();
            await ClearFloors(currentConferenceId());
            await sqlConnection.InsertOrReplaceAllAsync(floors);
        }

        public async Task CollectSpeakers()
        {
            List<Speaker> speakers = await Service.GetSpeakers();
            await ClearSpeakers(currentConferenceId());
            await sqlConnection.InsertOrReplaceAllAsync(speakers);
        }

        private async Task ClearEvents(string confId)
        {
            await sqlConnection.ExecuteAsync("delete from 'Event' where confId = '" + confId + "'");
        }

        private async Task ClearSpeakers(string confId)
        {
            await sqlConnection.ExecuteAsync("delete from 'Speaker' where confId = '" + confId + "'");
        }

        private async Task ClearFloors(string confId)
        {
            await sqlConnection.ExecuteAsync("delete from 'Floor' where confId = '" + confId + "'");
        }

        private async Task ClearTracks(string confId)
        {
            await sqlConnection.ExecuteAsync("delete from 'Track' where confId = '" + confId + "'");
        }

        private void DBConnection()
        {
            if (sqlConnection != null) return;

            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite");

            SQLiteConnectionString connString = new SQLiteConnectionString(path.ToString(), false);
            SQLiteConnectionWithLock connLock = new SQLiteConnectionWithLock(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), connString);

            sqlConnection = new SQLiteAsyncConnection(() => connLock);
        }

        public async Task<List<Track>> GetTracks()
        {
            string confId = currentConferenceId();
            return await sqlConnection.Table<Track>()
                .Where(t => t.confId.Equals(confId))
                .OrderBy(t => t.title)
                .ToListAsync();
        }

        public async Task<List<Speaker>> GetSpeakers()
        {
            string confId = currentConferenceId();
            return await sqlConnection.Table<Speaker>()
                .Where(s => s.confId.Equals(confId))
                .OrderBy(s => s.firstName)
                .ToListAsync();
        }

        public async Task<List<Speaker>> GetSpeakersBySearchCriteria(string searchString)
        {
            string confId = currentConferenceId();
            return await sqlConnection.Table<Speaker>()
                .Where(t => t.confId.Equals(confId) &&
                    (
                    (t.firstName != null && t.firstName.Contains(searchString)) ||
                    (t.lastName != null && t.lastName.Contains(searchString)) ||
                    (t.company != null && t.company.Contains(searchString))
                    )
                 )
                .OrderBy(s => s.firstName)
                .ToListAsync();
        }

        public async Task<Speaker> GetSpeaker(string uuid)
        {
            return await Service.GetSpeaker(uuid);
        }

        public async Task<List<Model.Floor>> GetFloors(string target)
        {
            string confId = currentConferenceId();
            return await sqlConnection.Table<Model.Floor>().Where(t => t.confId.Equals(confId) && t.target.Equals(target)).ToListAsync();
        }

        public async Task<Conference> GetCurrentConference()
        {
            string confId = currentConferenceId();
            return await sqlConnection.Table<Conference>().Where(t => t.id.Equals(confId)).FirstOrDefaultAsync();
        }

        public async Task<List<Event>> GetEventsByTrack(string trackId)
        {
            string confId = currentConferenceId();
            return await sqlConnection.Table<Event>().Where(t => t.confId.Equals(confId) && t.trackId.Equals(trackId)).ToListAsync();
        }

        public async Task<List<Event>> GetEventsByDay(string day)
        {
            string confId = currentConferenceId();
            return await sqlConnection.Table<Event>()
                .Where(t => t.confId.Equals(confId) && t.day.Equals(day))
                .OrderBy(e => e.fullTime)
                .ToListAsync();
        }

        public async Task<Event> GetEventsById(string id)
        {
            string confId = currentConferenceId();
            return await sqlConnection.Table<Event>().Where(t => t.confId.Equals(confId) && t.id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<Event>> GetEventsBySearchCriteria(string searchString)
        {
            string confId = currentConferenceId();
            return await sqlConnection.Table<Event>()
                .Where(t => t.confId.Equals(confId) && t.type.Equals(EventType.TALK) &&
                    (t.title.Contains(searchString) ||
                    t.summary.Contains(searchString) ||
                    t.speakerNames.Contains(searchString) ||
                    t.trackName.Contains(searchString)))
                .OrderByDescending(e => e.Starred)
                .OrderBy(e => e.title)
                .ToListAsync();
        }

        public async Task<List<Event>> GetStarredEvents()
        {
            string confId = currentConferenceId();
            return await sqlConnection.Table<Event>().Where(e => e.confId.Equals(confId) && e.Starred).ToListAsync();
        }

        private string currentConferenceId()
        {
            return (string)settings.Values["conferenceId"];
        }

        public async Task<Event> UpdateEvent(Event e)
        {
            await sqlConnection.UpdateAsync(e);
            return e;
        }

        public async Task<Note> SaveOrUpdateNote(Note note)
        {
            string confId = currentConferenceId();
            note.confId = confId;
            note.id = confId + note.talkId;
            await sqlConnection.InsertOrReplaceAsync(note);
            return note;
        }

        public async Task<Note> GetNote(string talkId)
        {
            string confId = currentConferenceId();
            Note note;
            List<Note> notes = await sqlConnection.Table<Note>().Where(n => n.talkId.Equals(talkId) && n.confId.Equals(confId)).ToListAsync();
            if (notes.Count == 0)
            {
                note = new Note();
                note.talkId = talkId;
            }
            else
            {
                note = notes.First();
            }
            return note;
        }

        public async Task<Vote> SaveOrUpdateVote(Vote vote)
        {
            string confId = currentConferenceId();
            vote.confId = confId;
            vote.id = confId + vote.talkId;
            await sqlConnection.InsertOrReplaceAsync(vote);
            return vote;
        }

        public async Task<Vote> GetVote(string talkId)
        {
            string confId = currentConferenceId();
            Vote vote;
            List<Vote> votes = await sqlConnection.Table<Vote>().Where(n => n.talkId.Equals(talkId) && n.confId.Equals(confId)).ToListAsync();
            if (votes.Count == 0)
            {
                vote = new Vote();
                vote.talkId = talkId;
            }
            else
            {
                vote = votes.First();
            }
            return vote;
        }
    }
}
