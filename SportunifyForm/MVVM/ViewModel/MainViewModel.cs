using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using SportunifyForm.MVVM.Model.QuickType;
using System.Collections.ObjectModel;

namespace SportunifyForm.MVVM.ViewModel
{
    internal class MainViewModel
    {
        public ObservableCollection<Item> Songs { get; set; }
        public MainViewModel()
        {
            Songs = new ObservableCollection<Item>();
            PopulateCollection();
        }

        void PopulateCollection()
        {
            //var client = new RestClient();
            //client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator("", "Bearer");

            //var request = new RestRequest("https://api.spotify.com/v1/browse/new-releases", Method.Get);
            //request.AddHeader("Accept", "application/json");
            //request.AddHeader("Content-type", "application/json");

            //var response = client.GetAsync(request).GetAwaiter().GetResult();

            //var data = JsonConvert.DeserializeObject<TrackModel>(response.Content);

            //for (int i = 0; i < data.Albums.Limit; i++)
            //{
            //    var Track = data.Albums.Items[i];
            //    Track.Duration = "2:32";
            //    Songs.Add(Track);
            //}

        }

    }
}
