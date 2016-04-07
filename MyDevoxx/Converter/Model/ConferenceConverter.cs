using MyDevoxx.Model;
using MyDevoxx.Services.RestModel;

namespace MyDevoxx.Converter.Model
{
    public class ConferenceConverter
    {
        public static Conference apply(CFP cfp)
        {
            Conference conference = new Conference();

            conference.id = cfp.id;
            conference.confType = cfp.confType;
            conference.confDescription = cfp.confDescription;
            conference.confIcon = cfp.confIcon;
            conference.venue = cfp.venue;
            conference.address = cfp.address;
            conference.country = cfp.country;
            conference.latitude = cfp.latitude;
            conference.longitude = cfp.longitude;
            conference.capacity = cfp.capacity;
            conference.sessions = cfp.sessions;
            conference.hashtag = cfp.hashtag;
            conference.splashImgURL = cfp.splashImgURL;
            conference.fromDate = cfp.fromDate;
            conference.toDate = cfp.toDate;
            conference.wwwURL = cfp.wwwURL;
            conference.regURL = cfp.regURL;
            conference.cfpURL = cfp.cfpURL;
            conference.talkURL = cfp.talkURL;
            conference.votingURL = cfp.votingURL;
            conference.votingEnabled = cfp.votingEnabled;
            conference.votingImageName = cfp.votingImageName;
            conference.cfpEndpoint = cfp.cfpEndpoint;
            conference.cfpVersion = cfp.cfpVersion;

            return conference;
        }
    }
}