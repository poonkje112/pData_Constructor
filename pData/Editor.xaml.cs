using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace pData
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : Window
    {
        Dictionary<string, string> _ImageFiles = new Dictionary<string, string>();
        string _CardImage;
        GithubUser _GitUser;
        Repository _Repository;
        string _Sha = null;

        public Editor(GithubUser githubUser, Repository repo)
        {
            InitializeComponent();
            _GitUser = githubUser;
            _Repository = repo;

            TryGetpData();
        }

        void TryGetpData()
        {
            using (WebClient client = new WebClient())
            {
                string url = $"https://raw.githubusercontent.com/{_Repository.Owner}/{_Repository.Name}/master/.pdata";
                try
                {
                    client.Headers = _GitUser.ConstructHeaders();
                    string response = client.DownloadString(url);
                    pDataConstructor data = JsonConvert.DeserializeObject<pDataConstructor>(response);

                    string teamText = "";
                    foreach (User value in data.TeamMembers.Values)
                    {
                        teamText += $"{value.Name}({value.Url}), ";
                    }

                    teamText = teamText.Substring(0, teamText.Length - 2);

                    //_ImageFiles = data.Images;
                    //ImageCount.Content = $"Image count: {data.Images.Count}";
                    projectDesc.Text = data.ProjectDescription;
                    StartedDesc.Text = data.ProjectStarted;
                    goalDesc.Text = data.ProjectGoal;
                    YouTubeVideo.Text = data.YouTubeVideo;
                    ShortDesc.Text = data.ShortDescription;
                    PlayUrl.Text = data.Demo;
                    LangAndInfo.Text = String.Join(", ", data.LangAndInfo);
                    TeamMembers.Text = teamText;

                    client.Headers = _GitUser.ConstructHeaders();
                    JArray contents = JArray.Parse(client.DownloadString($"{_Repository.Url}/contents/"));

                    //Get the sha of .pdata
                    for(int i = 0; i < contents.Count; i++)
                    {
                        if(contents[i]["name"].ToString() == ".pdata")
                        {
                            _Sha = contents[i]["sha"].ToString();
                            break;
                        }
                    }

                } catch(Exception ex)
                {
                    return;
                }
            }
        }

        private void ScreenshotBtn_Click(object sender, RoutedEventArgs e)
        {
            _ImageFiles.Clear();

            OpenFileDialog imagesDialog = new OpenFileDialog();
            imagesDialog.Multiselect = true;
            imagesDialog.Filter = "png files (*.png)|*.png";

            if (imagesDialog.ShowDialog() == true)
            {
                foreach (string file in imagesDialog.FileNames)
                {
                    _ImageFiles.Add(_ImageFiles.Count.ToString(), file);
                }
            }

            ImageCount.Content = $"Image count: {_ImageFiles.Count}";
        }

        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> base64Images = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> file in _ImageFiles)
            {
                base64Images.Add(base64Images.Count.ToString(), Convert.ToBase64String(File.ReadAllBytes(file.Value)));
            }
            string card64 = "";

            if (!string.IsNullOrEmpty(_CardImage))
            {
                card64 = Convert.ToBase64String(File.ReadAllBytes(_CardImage));
            }

            string[] team = TeamMembers.Text.Split(", ");
            string[] langAndInfo = LangAndInfo.Text.Split(", ");

            Dictionary<string, User> teamMembers = new Dictionary<string, User>();

            for (int i = 0; i < team.Length; i++)
            {
                team[i] = team[i].Replace(", ", "");
            }

            for(int i = 0; i < team.Length; i++)
            {
                int from = team[i].IndexOf("(") + "(".Length;
                int to = team[i].LastIndexOf(')');
                User user = new User()
                {
                    Url = team[i].Substring(from, to - from),
                    Name = team[i].Substring(0, from - 1)
                };

                teamMembers.Add(i.ToString(), user);
            }

            for (int i = 0; i < langAndInfo.Length; i++)
            {
                langAndInfo[i] = langAndInfo[i].Replace(", ", "");
            }

            pDataConstructor data = new pDataConstructor
            {
                Images = base64Images,
                Card = card64,
                ProjectDescription = projectDesc.Text,
                ProjectGoal = goalDesc.Text,
                ProjectStarted = StartedDesc.Text,
                YouTubeVideo = YouTubeVideo.Text,
                ShortDescription = ShortDesc.Text,
                GitSource = _Repository.RepoUrl,
                LangAndInfo = langAndInfo,
                TeamMembers = teamMembers,
                Demo = PlayUrl.Text,
                Name = _Repository.Name
            };

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            using (FileStream fs = new FileStream(@"C:\pdata_temp\.pdata", FileMode.Create, FileAccess.ReadWrite))
            {
                TextWriter writer = new StreamWriter(fs);
                writer.Write(json);

                writer.Flush();
                fs.Flush();
                writer.Close();
                fs.Close();
            }

            byte[] jsonBytes = Encoding.ASCII.GetBytes(json);
            string file64 = Convert.ToBase64String(jsonBytes);

            if (_GitUser.PushData(file64, _Repository, _Sha))
            {
                // Delete file
                File.Delete(@"C:\pdata_temp\.pdata");
            } else
            {
                return;
            }

            Close();
        }

        private void CardImgBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog imagesDialog = new OpenFileDialog();
            imagesDialog.Multiselect = false;
            imagesDialog.Filter = "png files (*.png)|*.png";

            if (imagesDialog.ShowDialog() == true)
            {
                _CardImage = imagesDialog.FileName;
            }
        }
    }
}
