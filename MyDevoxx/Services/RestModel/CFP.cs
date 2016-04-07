using System.Collections.Generic;

namespace MyDevoxx.Services.RestModel
{
    public class CFP
    {
        public string id { get; set; }
        public string confType { get; set; }
        public string confDescription { get; set; }
        public string confIcon { get; set; }
        public string venue { get; set; }
        public string address { get; set; }
        public string country { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public List<Floor> floors { get; set; }
        public string capacity { get; set; }
        public string sessions { get; set; }
        public string hashtag { get; set; }
        public string splashImgURL { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string wwwURL { get; set; }
        public string regURL { get; set; }
        public string cfpURL { get; set; }
        public string talkURL { get; set; }
        public string votingURL { get; set; }
        public string votingEnabled { get; set; }
        public string votingImageName { get; set; }
        public string cfpEndpoint { get; set; }
        public string cfpVersion { get; set; }
        public string youTubeURL { get; set; }
    }
}
