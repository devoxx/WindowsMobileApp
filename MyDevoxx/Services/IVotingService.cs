using MyDevoxx.Model;
using MyDevoxx.Services.RestModel.Voting;
using System.Threading.Tasks;

namespace MyDevoxx.Services
{
    public interface IVotingService
    {
        Task<VoteMessage> Vote(Vote vote);
    }
}
