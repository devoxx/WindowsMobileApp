using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDevoxx.Services.RestModel
{
    public class Talk
    {
        public string trackId { get; set; }
        public string talkType { get; set; }
        public string track { get; set; }
        public string summaryAsHtml { get; set; }
        public string id { get; set; }
        public List<SpeakerSimple> speakers { get; set; }
        public string title { get; set; }
        public string lang { get; set; }
        public string summary { get; set; }
    }
}
