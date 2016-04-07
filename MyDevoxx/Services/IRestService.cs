using MyDevoxx.Services.RestModel.Voting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyDevoxx.Services
{
    public interface IRestService
    {
        Task<List<Model.Event>> GetEvents(string day);

        Task<List<Model.Speaker>> GetSpeakers();

        Task<Model.Speaker> GetSpeaker(string uuid);

        Task<List<Model.Track>> GetTracks();

        Task<List<Model.Conference>> GetConferences();

        Task<List<Model.Floor>> GetFloors();

        Task<VoteMessage> VoteTalk(VoteBasic vote);

        Task<VoteMessage> VoteTalk(VoteReviews vote);

        Task<VoteResults> GetVoteResults(int limit, string day, string talkType, string track);

        Task<Categories> GetVoteCategories();
    }
}
