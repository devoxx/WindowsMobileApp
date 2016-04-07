using System.Runtime.Serialization;

namespace MyDevoxx.Services.RestModel.Voting
{
    [DataContract]
    public class VoteBasic
    {
        [DataMember]
        public int user { get; set; }
        [DataMember]
        public string talkId { get; set; }
        [DataMember]
        public int rating { get; set; }
    }
}
