using SQLite.Net.Attributes;

namespace MyDevoxx.Model
{
    public class Speaker
    {
        [PrimaryKey]
        public string uuid { get; set; }
        public string confId { get; set; }

        public string lastName { get; set; }
        public string firstName { get; set; }
        public string avatarURL { get; set; }
        public string bio { get; set; }
        public string bioAsHtml { get; set; }
        public string company { get; set; }
        public string blog { get; set; }
        public string twitter { get; set; }
        public string lang { get; set; }
        public string talkIds { get; set; }

        [Ignore]
        public string checkedBlog
        {
            get
            {
                if (blog != null && !blog.StartsWith("http://") && !blog.StartsWith("https://"))
                {
                    blog = "http://" + blog;
                }
                return blog;
            }
        }

        [Ignore]
        public string twitterLink
        {
            get
            {
                if (twitter != null)
                {
                    return "http://www.twitter.com/" + twitter;
                }
                return "";
            }
        }

        [Ignore]
        public string fullName
        {
            get
            {
                return firstName + " " + lastName;
            }
        }
    }
}
