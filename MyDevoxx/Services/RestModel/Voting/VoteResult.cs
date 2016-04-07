using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MyDevoxx.Services.RestModel.Voting
{
    [DataContract]
    public class VoteResult
    {
        [DataMember]
        public float avg { get; set; }
        [DataMember]
        public int count { get; set; }
        [DataMember]
        public int sum { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public List<string> speakers { get; set; }

        public string speaker
        {
            get
            {
                if(speakers == null)
                {
                    return "";
                }
                string speakerString = "";
                bool first = true;
                foreach (string s in speakers)
                {
                    if (!first)
                    {
                        speakerString += ", ";
                    }
                    speakerString += s;
                    first = false;
                }
                return speakerString;
            }
        }
    }
}
