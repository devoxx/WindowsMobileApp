using MyDevoxx.Model;

namespace MyDevoxx.Converter.Model
{
    public class SpeakerConverter
    {
        public static Speaker apply(Services.RestModel.Speaker s, string confId)
        {
            Speaker speaker = new Speaker();

            speaker.confId = confId;
            speaker.firstName = s.firstName;
            speaker.lastName = s.lastName;
            speaker.uuid = s.uuid;
            speaker.blog = s.blog;
            speaker.bio = s.bio;
            speaker.bioAsHtml = s.bioAsHtml;
            speaker.company = s.company;
            speaker.twitter = s.twitter;
            speaker.lang = s.lang;

            if (s.avatarURL != null && s.avatarURL != "")
            {
                speaker.avatarURL = s.avatarURL;
            }
            else
            {
                speaker.avatarURL = @"ms-appx:///Assets/speaker_placeholder.png";
            }

            if (s.acceptedTalks != null)
            {
                speaker.talkIds = "";
                foreach (Services.RestModel.AcceptedTalk a in s.acceptedTalks)
                {
                    speaker.talkIds += a.id + ",";
                }
                if (speaker.talkIds.Length > 0)
                {
                    speaker.talkIds = speaker.talkIds.Remove(speaker.talkIds.Length - 1);
                }
            }

            return speaker;
        }
    }
}
