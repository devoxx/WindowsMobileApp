using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyDevoxx.Services.RestModel.Voting
{
    [DataContract]
    public class VoteReviews
    {
        [DataMember]
        public int user { get; set; }
        [DataMember]
        public string talkId { get; set; }
        [DataMember]
        public List<VoteDetail> details { get; set; }
    }
}
