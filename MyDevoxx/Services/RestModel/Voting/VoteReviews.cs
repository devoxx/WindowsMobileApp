using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyDevoxx.Services.RestModel.Voting
{
    [DataContract]
    public class VoteReviews
    {
        [DataMember]
        public string user { get; set; }
        [DataMember]
        public string talkId { get; set; }
        [DataMember]
        public List<VoteDetail> details { get; set; }
    }
}
