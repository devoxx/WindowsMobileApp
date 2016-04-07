using System.Runtime.Serialization;

namespace MyDevoxx.Services.RestModel.Voting
{
    [DataContract]
    public class VoteDetail
    {
        [DataMember]
        public int rating { get; set; }
        [DataMember]
        public string aspect { get; set; }
        [DataMember]
        public string review { get; set; }
    }
}
