using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MyDevoxx.Services.RestModel.Voting
{
    [DataContract]
    public class Categories
    {
        [DataMember]
        public List<string> talkTypes { set; get; }
        [DataMember]
        public List<string> tracks { set; get; }
    }
}
