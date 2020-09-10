using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace pData
{
    public class GithubUser
    {
        string _Email;
        string _Username;
        string _Token;
        WebHeaderCollection _Headers;

        private GithubUser(string email, string username, string token)
        {
            _Email = email;
            _Username = username;
            _Token = token;

            _Headers = new WebHeaderCollection();
            _Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:80.0) Gecko/20100101 Firefox/80.0");
            _Headers.Add("Authorization", $"token {token}");
            _Headers.Add("content-type", "application/json");
        }

        public static GithubUser CreateAndCheckUser(string email, string username, string token)
        {
            if (Validate(token)) return new GithubUser(email, username, token);
            return null;
        }

        static bool Validate(string token)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:80.0) Gecko/20100101 Firefox/80.0");
                client.Headers.Add("Authorization", $"token {token}");
                client.Headers.Add("content-type", "application/json");
                JObject data = JObject.Parse(client.DownloadString("https://api.github.com/user"));
                return !string.IsNullOrEmpty(data["login"].ToString());
            }
        }

        public Repository[] GetRepos()
        {
            Repository[] repos;
            using (WebClient client = new WebClient())
            {
                client.Headers = _Headers;
                JArray data = JArray.Parse(client.DownloadString("https://api.github.com/user/repos"));
                repos = new Repository[data.Count];

                for(int i = 0; i < repos.Length; i++)
                {
                    JObject obj = (JObject)data[i];
                    repos[i] = new Repository(obj["owner"]["login"].ToString(), obj["name"].ToString(), obj["url"].ToString());
                }
            }

            return repos;
        }
    }
}
