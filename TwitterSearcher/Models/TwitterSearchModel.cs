using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Windows.Data.Json;
using Windows.UI.Xaml.Data;
using W8Shared;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml.Input;
using Windows.UI.StartScreen;
using System.Collections.Specialized;
using TwitterSearcher.ViewModels;
using Windows.UI.Xaml;

namespace TwitterSearcher.Models
{
    public class TwitterSearchModel
    {
        public string SearchTerm { get; set; }
        public string Since { get; set; }
        public BindableObservableCollection<Tweet> Tweets { get; set; }

        public TwitterSearchModel(string searchTerm)
        {
            Since = "1";
            SearchTerm = searchTerm;
            Tweets = new BindableObservableCollection<Tweet>();
            Update();
        }

        //note the async keyword - it's cause we're using an "await" method in our method body
        public async void Update()
        {
            var client = new HttpClient();
            //ooh fancy new await syntax!
            //this actually executes in parallel and pauses the rest of the method's execution until it returns
            //(in a non blocking way of course)
            var result = await client.GetAsync(string.Format("http://search.twitter.com/search.json?q={0}&since_id={1}", SearchTerm, Since));
            var root = new JsonObject(result.Content.ReadAsString());
            var array = root.GetNamedArray("results");
            for(int i = 0; i < array.Count; i++) 
            {
                var element = array[i];
                JsonObject obj = element.GetObject();
                Tweet tweet = new Tweet
                {
                    created_at = obj.GetNamedString("created_at"),
                    from_user = obj.GetNamedString("from_user"),
                    id_str = obj.GetNamedString("id_str"),
                    profile_image_url = obj.GetNamedString("profile_image_url"),
                    text = obj.GetNamedString("text")
                };
                if (!Tweets.Any(t => t.id_str == tweet.id_str))
                {
                    Tweets.Insert(0, tweet);
                }
            }
        }
    }

    public class Tweet
    {
        public string created_at { get; set; }
        public string from_user { get; set; }
        public string id_str { get; set; }
        public string profile_image_url { get; set; }
        public string text { get; set; }
    }

} 
