using MyDevoxx.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyDevoxx.Services
{
    public delegate void FinishedLoading(bool successful);

    public interface IDataService
    {
        Task<List<Conference>> FetchConferences();

        void CollectAll(FinishedLoading callback);

        void UpdateAll(FinishedLoading callback);

        Task<bool> CollectEvents();

        Task CollectTracks();

        Task CollectFloors();

        Task CollectSpeakers();

        Task<List<Track>> GetTracks();

        Task<List<Speaker>> GetSpeakers();

        Task<List<Speaker>> GetSpeakersBySearchCriteria(string searchString);

        Task<Speaker> GetSpeaker(string uuid);

        Task<List<Model.Floor>> GetFloors(string target);

        Task<Conference> GetCurrentConference();

        Task<List<Event>> GetEventsByTrack(string trackId);

        Task<List<Event>> GetEventsByDay(string day);

        Task<Event> GetEventsById(string id);

        Task<List<Event>> GetEventsBySearchCriteria(string searchString);

        Task<List<Event>> GetStarredEvents();

        Task<Event> UpdateEvent(Event e);

        Task<Note> SaveOrUpdateNote(Note note);

        Task<Note> GetNote(string talkId);

        Task<Vote> SaveOrUpdateVote(Vote vote);

        Task<Vote> GetVote(string talkId);
    }
}
