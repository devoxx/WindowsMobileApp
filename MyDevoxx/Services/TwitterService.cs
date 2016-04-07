using MyDevoxx.Model;
using System;
using System.Net;
using Windows.Storage;
using Windows.System;

namespace MyDevoxx.Services
{
    public class TwitterService : ITwitterService
    {
        private static string twitterBaseUri = "https://twitter.com";
        private static string twitterTweetUri = twitterBaseUri + "/intent/tweet?text=";
        private static string twitterSearchUri = twitterBaseUri + "/search?q=";


        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

        public async void sendSpeakerTweet(Speaker s)
        {
            await Launcher.LaunchUriAsync(new Uri(twitterTweetUri + s.twitter));
        }

        public async void sendTalkTweet(Event e)
        {
            string talkURL = (string)settings.Values["talkURL"];
            string msg = WebUtility.UrlEncode(e.title + " " + talkURL + e.id);
            await Launcher.LaunchUriAsync(new Uri(twitterTweetUri + msg));
        }

        public async void goToHashTag(string hashTag)
        {
            await Launcher.LaunchUriAsync(new Uri(twitterSearchUri + WebUtility.UrlEncode(hashTag)));
        }
    }
}
