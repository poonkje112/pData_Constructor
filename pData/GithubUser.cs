﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace pData
{
    public class GithubUser
    {
        string _Email;
        string _Username;
        WebHeaderCollection _Headers;

        private GithubUser(string email, string username, string token)
        {
            _Email = email;
            _Username = username;

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

                JObject data;

                try
                {
                    data = JObject.Parse(client.DownloadString("https://api.github.com/user"));
                }
                catch (WebException ex)
                {
                    switch (ex.Status)
                    {
                        case WebExceptionStatus.ProtocolError:
                            MessageBox.Show("Invalid Token!", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;

                        default:
                            MessageBox.Show(ex.Message, "Unknown Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                    }

                    return false;
                }

                return !string.IsNullOrEmpty(data["login"].ToString());
            }
        }

        public Repository[] GetRepos()
        {
            Repository[] repos;
            using (WebClient client = new WebClient())
            {
                client.Headers = ConstructHeaders();
                string json = client.DownloadString("https://api.github.com/user/repos");
                JArray data = JArray.Parse(json);
                repos = new Repository[data.Count];

                for (int i = 0; i < repos.Length; i++)
                {
                    JObject obj = (JObject)data[i];
                    repos[i] = new Repository(obj["owner"]["login"].ToString(), obj["name"].ToString(), obj["url"].ToString(), (bool)obj["private"]);
                }
            }

            return repos;
        }

        WebHeaderCollection ConstructHeaders()
        {
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Set("User-Agent", _Headers.Get("User-Agent"));
            headers.Set("content-type", _Headers.Get("content-type"));
            headers.Set("Authorization", _Headers.Get("Authorization"));

            return headers;
        }

        //TODO Add SHA for updating file
        public bool PushData(string data, Repository repo)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers = ConstructHeaders();

                JObject json = new JObject()
                {
                    ["message"] = "Added/Updated .pdata file",
                    ["content"] = data,
                    ["committer"] = new JObject()
                    {
                        ["name"] = _Username,
                        ["email"] = _Email
                    }
                };

                string response = client.UploadString($"https://api.github.com/repos/{repo.Owner}/{repo.Name}/contents/.pdata", "PUT", json.ToString());
            }

            return true;
        }
    }
}
