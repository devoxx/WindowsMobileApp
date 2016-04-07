using MyDevoxx.Model;

namespace MyDevoxx.Services
{
    interface ITwitterService
    {
        void sendTalkTweet(Event e);

        void sendSpeakerTweet(Speaker s);

        void goToHashTag(string hashTag);
    }
}
