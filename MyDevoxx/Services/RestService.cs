using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Web.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MyDevoxx.Services.RestModel;
using Windows.Storage;
using System.Threading;
using MyDevoxx.Converter.Model;
using SQLite.Net;
using SQLite.Net.Async;
using MyDevoxx.Services.RestModel.Voting;

namespace MyDevoxx.Services
{
    public class RestService : IRestService
    {
        private CancellationTokenSource cts;
        private StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        private ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        private SQLiteAsyncConnection sqlConnection;

        private string cfpUrl = "https://s3-eu-west-1.amazonaws.com/cfpdevoxx/cfp.json";
        private string speakersUrl = "/speakers";
        private string tracksUrl = "/tracks";
        private string scheduleUrl = "/schedules";

        public RestService()
        {
            cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            DBConnection();
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            // Add a user-agent header 
            var headers = client.DefaultRequestHeaders;
            headers.UserAgent.ParseAdd("ie");
            headers.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            headers.Accept.ParseAdd("application/json");

            return client;
        }

        public async Task<List<Model.Event>> GetEvents(string day)
        {
            string endpoint = (string)settings.Values["cfpEndpoint"];
            string confId = (string)settings.Values["conferenceId"];
            string country = (string)settings.Values["country"];
            string url = endpoint + "/conferences" + "/" + confId + scheduleUrl + "/" + day + "/";
            Schedule schedule = await fetch<Schedule>(url, "schedule_" + day + "_" + confId + ".json");

            return EventConverter.apply(schedule, confId, country);
        }

        public async Task<List<Model.Speaker>> GetSpeakers()
        {
            string endpoint = (string)settings.Values["cfpEndpoint"];
            string confId = (string)settings.Values["conferenceId"];
            string url = endpoint + "/conferences" + "/" + confId + speakersUrl;
            List<Speaker> result = await fetch<List<Speaker>>(url, "speakerlist_" + confId + ".json");
            List<Model.Speaker> speakerList = new List<Model.Speaker>();
            foreach (Speaker s in result)
            {
                speakerList.Add(SpeakerConverter.apply(s, confId));
            }
            return speakerList;
        }

        public async Task<Model.Speaker> GetSpeaker(string uuid)
        {
            string endpoint = (string)settings.Values["cfpEndpoint"];
            string confId = (string)settings.Values["conferenceId"];
            string url = endpoint + "/conferences" + "/" + confId + speakersUrl + "/" + uuid;
            Speaker speaker = await fetch<Speaker>(url, "speaker_" + uuid + "_" + confId + ".json");
            return SpeakerConverter.apply(speaker, confId);
        }

        public async Task<List<Model.Track>> GetTracks()
        {
            string endpoint = (string)settings.Values["cfpEndpoint"];
            string confId = (string)settings.Values["conferenceId"];
            string url = endpoint + "/conferences" + "/" + confId + tracksUrl;
            TrackList result = await fetch<TrackList>(url, "tracksList_" + confId + ".json");

            return TrackListConverter.apply(result, confId);
        }

        public async Task<List<Model.Conference>> GetConferences()
        {
            List<CFP> cfp = await fetch<List<CFP>>(cfpUrl, "cfp.json");
            List<Model.Conference> conferences = new List<Model.Conference>();
            foreach (CFP c in cfp)
            {
                conferences.Add(ConferenceConverter.apply(c));
            }

            return conferences;
        }

        public async Task<List<Model.Floor>> GetFloors()
        {
            string confId = (string)settings.Values["conferenceId"];
            List<CFP> cfp = await fetch<List<CFP>>(cfpUrl, "cfp.json");
            foreach (CFP c in cfp)
            {
                if (c.id.Equals(confId))
                {
                    return FloorConverter.apply(c);
                }
            }

            return new List<Model.Floor>(); ;
        }

        private async Task<T> fetch<T>(string url, string cacheFileName)
        {
            Debug.WriteLine("Requested URL: " + url);

            HttpClient client = CreateHttpClient();

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            Uri resourceUri = new Uri(url, UriKind.Absolute);
            string responseText;
            Model.ETag eTag;

            client.DefaultRequestHeaders.Remove("If-None-Match");
            eTag = await GetEtag(url);
            if (eTag != null)
            {
                client.DefaultRequestHeaders.Add(new KeyValuePair<string, string>("If-None-Match", eTag.eTag));
            }

            try
            {
                if (eTag == null || (DateTime.Now - eTag.timeStamp).TotalMinutes >= 5)
                {
                    //last request was more than 5 minutes ago
                    HttpResponseMessage msg = await client.GetAsync(resourceUri);//.AsTask(cts.Token);
                    if (HttpStatusCode.NotModified.Equals(msg.StatusCode))
                    {
                        //content has not changed since last request
                        responseText = await loadFromFile(cacheFileName);
                        SaveEtag(url, eTag.eTag);
                    }
                    else {
                        //content is new or has changed since last request
                        responseText = await msg.Content.ReadAsStringAsync();
                        await saveToFile(cacheFileName, responseText);

                        string newEtag;
                        msg.Headers.TryGetValue("Etag", out newEtag);
                        Debug.WriteLine(eTag);
                        SaveEtag(url, newEtag);
                    }
                }
                else
                {
                    //last request was less than 5 minutes ago
                    responseText = await loadFromFile(cacheFileName);
                }

                return (T)serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(responseText)));
            }
            catch (Exception e)
            {
                Debug.WriteLine("=> " + e.Message);
                responseText = await loadFromFile(cacheFileName);
                if (responseText == null)
                {
                    responseText = await loadFromFallbackFile(cacheFileName);
                    Debug.WriteLine("Read fallback file!");
                }
                if (responseText != null)
                {
                    return (T)serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(responseText)));
                }
            }
            return Activator.CreateInstance<T>();
        }

        private async Task saveToFile(string filename, string data)
        {
            StorageFile file = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(file, data);
        }

        private async Task<string> loadFromFile(string filename)
        {
            try
            {
                StorageFile file = await localFolder.GetFileAsync(filename);
                return await FileIO.ReadTextAsync(file);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<string> loadFromFallbackFile(string filename)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///Assets/FallbackData/" + filename));
                return await FileIO.ReadTextAsync(file);
            }
            catch (Exception)
            {
                Debug.WriteLine("Fallback file not found " + filename);
                return null;
            }
        }

        private async Task<Model.ETag> GetEtag(string url)
        {
            Model.ETag result = await sqlConnection.Table<Model.ETag>().Where(i => i.url.Equals(url)).FirstOrDefaultAsync();
            if (result != null)
            {
                Debug.WriteLine(result.eTag + " - " + result.url + " created at " + result.timeStamp);
                return result;
            }
            return null;
        }

        private async void SaveEtag(string url, string eTag)
        {
            await sqlConnection.InsertOrReplaceAsync(new Model.ETag() { url = url, eTag = eTag, timeStamp = DateTime.Now });
        }

        private void DBConnection()
        {
            if (sqlConnection != null) return;

            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite");

            SQLiteConnectionString connString = new SQLiteConnectionString(path.ToString(), false);
            SQLiteConnectionWithLock connLock = new SQLiteConnectionWithLock(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), connString);

            sqlConnection = new SQLiteAsyncConnection(() => connLock);
        }

        public async Task<VoteMessage> VoteTalk(VoteBasic vote)
        {
            return await Vote(vote);
        }

        public async Task<VoteMessage> VoteTalk(VoteReviews vote)
        {
            return await Vote(vote);
        }

        private async Task<VoteMessage> Vote<T>(T vote)
        {
            HttpClient client = CreateHttpClient();

            string confId = (string)settings.Values["conferenceId"];
            string votingURL = (string)settings.Values["votingURL"];

            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            serializer.WriteObject(stream, vote);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            string dataString = sr.ReadToEnd();

            try
            {
                HttpResponseMessage response = await client.PostAsync(new Uri(votingURL + "api/voting/v1/vote"), new HttpStringContent(dataString, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json"));
                if (!HttpStatusCode.Created.Equals(response.StatusCode))
                {
                    string responseTxt = await response.Content.ReadAsStringAsync();
                    DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(VoteMessage));
                    VoteMessage message = obj.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(responseTxt))) as VoteMessage;
                    return message;
                }
            }
            catch (Exception)
            {
                VoteMessage message = new VoteMessage();
                message.message = "Could not reach server, please try again later.";
                return message;
            }
            return null;
        }

        public async Task<VoteResults> GetVoteResults(int limit, string day, string talkType, string track)
        {
            //[/DV15/top/talks{?limit}&{day}&{talkType}&{track}]
            HttpClient client = CreateHttpClient();
            string confId = (string)settings.Values["conferenceId"];
            string url = (string)settings.Values["votingURL"];
            string parameters = buildVoteParameters(limit, day, talkType, track);
            try
            {
                HttpResponseMessage response = await client.GetAsync(new Uri(url + confId + "/top/talks" + parameters));
                if (HttpStatusCode.Ok.Equals(response.StatusCode))
                {
                    string responseTxt = await response.Content.ReadAsStringAsync();
                    DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(VoteResults));
                    VoteResults result = obj.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(responseTxt))) as VoteResults;
                    return result;
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Could not reach server to get vote results!");
            }
            return new VoteResults();
        }

        private string buildVoteParameters(int limit, string day, string talkType, string track)
        {
            StringBuilder b = new StringBuilder("");
            if (limit > 0)
            {
                b.Append("?limit=").Append(limit);
            }
            if (!String.IsNullOrEmpty(day))
            {
                b.Append(getConChar(b)).Append("day=").Append(day);
            }
            if (!String.IsNullOrEmpty(talkType))
            {

                b.Append(getConChar(b)).Append("talkType=").Append(System.Net.WebUtility.UrlEncode(talkType));
            }
            if (!String.IsNullOrEmpty(track))
            {
                b.Append(getConChar(b)).Append("track=").Append(System.Net.WebUtility.UrlEncode(track));
            }

            return b.ToString();
        }

        private string getConChar(StringBuilder b)
        {
            if (b.Length == 0)
            {
                return "?";
            }
            else
            {
                return "&";
            }
        }

        public async Task<Categories> GetVoteCategories()
        {
            HttpClient client = CreateHttpClient();
            string confId = (string)settings.Values["conferenceId"];
            string url = (string)settings.Values["votingURL"];
            try
            {
                HttpResponseMessage response = await client.GetAsync(new Uri(url + confId + "/categories"));
                if (HttpStatusCode.Ok.Equals(response.StatusCode))
                {
                    string responseTxt = await response.Content.ReadAsStringAsync();
                    DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(Categories));
                    Categories result = obj.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(responseTxt))) as Categories;
                    return result;
                }
            }
            catch (Exception)
            { }
            return null;
        }

        //public async Task<VoteResult> getVoteForTalk(string talkId)
        //{
        //    string conference = (string)settings.Values["conference"];
        //    string url = Config.Instance.getConference(conference).voteUrl;
        //    try
        //    {
        //        HttpResponseMessage response = await httpClient.GetAsync(new Uri(url + voteResultTalkAddress));
        //        if (HttpStatusCode.Ok.Equals(response.StatusCode))
        //        {
        //            string responseTxt = await response.Content.ReadAsStringAsync();
        //            DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(VoteResult));
        //            VoteResult result = obj.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(responseTxt))) as VoteResult;
        //            return result;
        //        }
        //    }
        //    catch (Exception)
        //    { }
        //    return null;
        //}
    }
}
