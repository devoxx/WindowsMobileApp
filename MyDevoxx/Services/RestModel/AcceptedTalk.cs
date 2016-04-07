using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDevoxx.Services.RestModel
{
    public class AcceptedTalk
    {
        public string talkType { get; set; }
        public string track { get; set; }
        public List<Link> links { get; set; }
        public string id { get; set; }
        public string title { get; set; }
    }
}
