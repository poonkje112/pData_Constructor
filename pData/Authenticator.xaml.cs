using System;
using System.Collections.Generic;
using System.Text;
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
    /// Interaction logic for Authenticator.xaml
    /// </summary>
    public partial class Authenticator : Window
    {
        public GithubUser User;
        
        public Authenticator()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_Email.Text) || string.IsNullOrEmpty(_Username.Text) || string.IsNullOrEmpty(_Token.Password)) return;
            User = GithubUser.CreateAndCheckUser(_Email.Text, _Username.Text, _Token.Password);
            if (User == null) return;
            this.Close();
        }
    }
}
