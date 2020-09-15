using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GithubUser _GitUser;
        List<Repository> _Repos;

        public MainWindow()
        {
            InitializeComponent();

            if (_GitUser == null)
            {
                Authenticator auth = new Authenticator();
                auth.ShowDialog();
                _GitUser = auth.User;
            }

            if (_GitUser == null)
            {
                Close();
                return;
            }

            Refresh();
        }

        private void Datagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Datagrid.SelectedIndex < 0 || Datagrid.SelectedIndex > _Repos.Count) return;

            Repository selectedRepo = _Repos[Datagrid.SelectedIndex];

            Editor editor = new Editor(_GitUser, _Repos[Datagrid.SelectedIndex]);
            editor.Name.Content = selectedRepo.FullName;
            editor.ShowDialog();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            Refresh(HideReposCheckBox.IsChecked.Value);
        }

        void Refresh(bool hidePrivate = false)
        {
            if (Datagrid.Items.Count > 0)
            {
                Datagrid.Items.Clear();
            }

            _Repos = _GitUser.GetRepos().ToList();

            // Removes all private repositories
            if (hidePrivate)
            {
                for (int i = _Repos.Count - 1; i >= 0; i--)
                {
                    if (_Repos[i].IsPrivate) _Repos.RemoveAt(i);
                }
            }


            for (int i = 0; i < _Repos.Count; i++)
            {
                Datagrid.Items.Add(_Repos[i]);
            }
        }

        private void HideReposCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Refresh(HideReposCheckBox.IsChecked.Value);
        }
    }
}