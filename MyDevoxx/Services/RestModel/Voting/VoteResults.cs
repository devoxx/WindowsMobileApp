using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MyDevoxx.Services.RestModel.Voting
{
    [DataContract]
    public class VoteResults
    {
        [DataMember]
        public List<VoteResult> talks { get; set; }
    }
}
