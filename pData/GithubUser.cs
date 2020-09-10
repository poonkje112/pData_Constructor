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

        private GithubUser(string email, string username, string token)
        {
            _Email = email;
            _Username = username;
            _Token = token;
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
                client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:80.0) Gecko/20100101 Firefox/80.0");
                client.Headers.Add("Authorization", $"token {token}");
                JObject data = JObject.Parse(client.DownloadString("https://api.github.com/user"));
                return !string.IsNullOrEmpty(data["login"].ToString());
            }
        }
    }
}
