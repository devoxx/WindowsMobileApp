using System.Runtime.Serialization;

namespace MyDevoxx.Services.RestModel.Voting
{
    [DataContract]
    public class VoteMessage
    {
        [DataMember]
        public string message { get; set; }

        [DataMember]
        public string messaage { get; set; }
    }
}
