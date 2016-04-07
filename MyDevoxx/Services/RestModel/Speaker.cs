using System.Collections.Generic;

namespace MyDevoxx.Services.RestModel
{
    public class Speaker
    {
        public string uuid { get; set; }
        public string lastName { get; set; }
        public List<Link> links { get; set; }
        public string firstName { get; set; }
        public string avatarURL { get; set; }
        public string twitter { get; set; }
        public string lang { get; set; }
        public string blog { get; set; }
        public string company { get; set; }
        public string bio { get; set; }
        public string bioAsHtml { get; set; }
        public List<AcceptedTalk> acceptedTalks { get; set; }
    }
}
