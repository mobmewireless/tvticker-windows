using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TV_Ticker
{
    public class JsonData
    {
        public string jsonrpc { get; set; }

        public string id { get; set; }

        public string method { get; set; }

        public List<string> Params { get; set; }

        public JsonData()
        {
            jsonrpc = "2.0";
            id = "1";
            method = "Ping";
            Params = new List<string>() { "1", "2", "3" };
        }
    }

    public class JsonResponse
    {
        public string jsonrpc { get; set; }

        public string id { get; set; }

        public List<object> result { get; set; }

        public string error { get; set; }
    }

    public class JsonWorker
    {
        public static Dictionary<int, string> Channels = new Dictionary<int, string>();
        public static Dictionary<int, string> Categories = new Dictionary<int, string>();
        public static string Serialize(object objForSerialization)
        {
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(objForSerialization.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, objForSerialization);
            string json = UTF8Encoding.UTF8.GetString(ms.ToArray(), 0, ms.ToArray().Length);
            return json;
        }

        public static object Deserialize(string jsonString)
        {
            JObject o = JObject.Parse(jsonString);
            return o;
        }

        public static string generateJsonString(JsonData myJsonData)
        {
            // serialization
            string jsonString = Serialize(myJsonData);
            return jsonString.ToLower();
        }

        public static JObject generateJsonData(string myJsonString)
        {
            JObject myJsonObject = (JObject)Deserialize(myJsonString);
            return myJsonObject;
        }

        public static void makeJsonRequest(String url, JsonData myJsonData)
        {
            WebClient client = new WebClient();
            String jsonString = generateJsonString(myJsonData);
            client.UploadStringAsync(new Uri(url), "POST", jsonString);
            client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_UploadStringCompleted);
        }

        public static void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            string Result = e.Result;
            JObject myJSONObject = generateJsonData(Result);
            switch ((string)myJSONObject["id"])
            {
                case "channels_list": AddUpdateChannels(myJSONObject); break;
                case "categories_list": AddUpdateCategories(myJSONObject); break;
                case "programs_list": UpdatePrograms(myJSONObject); break;
            }
        }

        private static void AddUpdateCategories(JObject myJSONObject)
        {
            JArray ResultSet = (JArray)myJSONObject["result"];
            App.ViewModel.Categories.Clear();
            foreach (JObject myItem in ResultSet)
            {
                JObject mySubItem = (JObject)myItem["category"];
                string categoryName = (string)mySubItem["name"];
                int id = (int)mySubItem["id"];
                App.ViewModel.Categories.Add(new CategoryViewModel() { CategoryName = categoryName });
                Categories.Add(id, categoryName);
            }
        }

        private static void UpdatePrograms(JObject myJSONObject)
        {
            JArray ResultSet = (JArray)myJSONObject["result"];
            App.ViewModel.Showing.Clear();
            App.ViewModel.Schedule.Clear();
            foreach (JObject myItem in ResultSet)
            {
                JObject mySubItem = (JObject)myItem["program"];
                string programName = (string)mySubItem["name"];
                DateTime air_time_start = (DateTime)mySubItem["air_time_start"];
                DateTime air_time_end = (DateTime)mySubItem["air_time_end"];
                int channel_id = (int)mySubItem["channel_id"];
                DateTime now = DateTime.UtcNow;
                air_time_start = DateTime.SpecifyKind(air_time_start, DateTimeKind.Utc);
                air_time_end = DateTime.SpecifyKind(air_time_end, DateTimeKind.Utc);
                string imageSource = "http://admin.tvticker.in/image/" + ((Int32)mySubItem["thumbnail_id"]).ToString() + "/profile";
                if (air_time_start <= now && air_time_end >= now)
                {
                    air_time_start = air_time_start.ToLocalTime();
                    air_time_end = air_time_end.ToLocalTime();
                   App.ViewModel.Showing.Add(new ItemViewModel()
                  {
                      LineOne = programName,
                      LineTwo = Channels[channel_id] + " (" + air_time_start.Hour.ToString() + ":" + air_time_start.Minute.ToString() + " - " + air_time_end.Hour.ToString() + ":" + air_time_end.Minute.ToString() + ")",
                      ImgSource = imageSource
                  });
                }
                else
                {
                    air_time_start = air_time_start.ToLocalTime();
                    air_time_end = air_time_end.ToLocalTime();
                    App.ViewModel.Schedule.Add(new ItemViewModel()
                    {
                        LineOne = programName,
                        LineTwo = Channels[channel_id] + " (" + air_time_start.Hour.ToString() + ":" + air_time_start.Minute.ToString() + " - " + air_time_end.Hour.ToString() + ":" + air_time_end.Minute.ToString() + ")",
                        ImgSource = imageSource
                    });
                }
            }
        }
            

        public static void AddUpdateChannels(JObject myJSONObject)
        {
            JArray ResultSet = (JArray)myJSONObject["result"];
            App.ViewModel.Channels.Clear();
            foreach (JObject myItem in ResultSet)
            {
                JObject mySubItem = (JObject)myItem["channel"];
                string channelName = (string)mySubItem["name"];
                int id = (int)mySubItem["id"];
                App.ViewModel.Channels.Add(new ChannelViewModel() { ChannelName = channelName });
                Channels.Add(id, channelName);
            }
        }
    }
}
