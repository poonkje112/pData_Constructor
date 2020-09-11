using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        List<string> _ImageFiles = new List<string>();
        GithubUser _GitUser;
        Repository _Repository;

        public Editor(GithubUser githubUser, Repository repo)
        {
            InitializeComponent();
            _GitUser = githubUser;
            _Repository = repo;
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
                    _ImageFiles.Add(file);
                }
            }

            ImageCount.Content = $"Image count: {_ImageFiles.Count}";
        }

        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            List<string> base64Images = new List<string>();
            foreach (string file in _ImageFiles)
            {
                base64Images.Add(Convert.ToBase64String(File.ReadAllBytes(file)));
            }


            pDataConstructor data = new pDataConstructor
            {
                Images = base64Images.ToArray(),
                ProjectDescription = projectDesc.Text,
                ProjectGoal = goalDesc.Text,
                ProjectStarted = StartedDesc.Text,
                YouTubeVideo = YouTubeVideo.Text
            };

            string json = JsonConvert.SerializeObject(data);

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

            if(_GitUser.PushData(file64, _Repository))
            {
                // Delete file
                File.Delete(@"C:\pdata_temp\.pdata");
            }

            Close();
        }
    }
}
