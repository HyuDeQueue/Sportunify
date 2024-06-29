using Newtonsoft.Json;
using Repositories.Entities;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    internal class MainView
    {
        public ObservableCollection<Item> Songs { get; set; }

        public MainView()
        {
            Songs = new ObservableCollection<Item>();
        }

        void PopulateCollectoin()
        {
            var client = new RestClient();
            client Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator("1POdFZRZbvb...qqillRxMr2z", "Bearer");

            var request = new RestRequest("https://api.spotify.com/v1/browse/new-releases", Method.Get);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");

            var response = client.PostAsync(request).GetAwaiter().GetResult();
            var data = JsonConvert.DeserializeObject<Item>(response.Content);
        }
    }
}
