using Newtonsoft.Json;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace SpotifyAPI.Web.Auth
{
    public class ClientCredentialsAuth
    {
        public Scope Scope { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        /// <summary>
        ///     Starts the auth process and
        /// </summary>
        /// <returns>A new Token</returns>
        public Token DoAuth()
        {

            using (HttpClient hp = new HttpClient())
            {
                hp.DefaultRequestHeaders.Add(
                    "Authorization",
                    "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientId + ":" + ClientSecret))
                    );

                Dictionary<string, string> col = new Dictionary<string, string>
                {
                    {"grant_type", "client_credentials"},
                    {"scope", Scope.GetStringAttribute(" ")}
                };

                byte[] data;
                try
                {
                    var dataTask = hp.PostAsync(new Uri("https://accounts.spotify.com/api/token/"), new FormUrlEncodedContent(col));
                    data = dataTask.Result.Content.ReadAsByteArrayAsync().Result; //The original API wasn't asynchronous either so I've chosen blocking way ATM
                }
                catch (WebException e)
                {
                    using (StreamReader reader = new StreamReader(e.Response.GetResponseStream()))
                    {
                        data = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                    }
                }
                return JsonConvert.DeserializeObject<Token>(Encoding.UTF8.GetString(data));
            }
        }
    }
}