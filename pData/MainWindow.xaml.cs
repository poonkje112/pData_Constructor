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

		public MainWindow()
		{
			InitializeComponent();
			if (_GitUser == null)
			{
				Authenticator auth = new Authenticator();
				auth.ShowDialog();
				_GitUser = auth.User;
			}
			
			if(_GitUser == null) Close();

			Repository[] repos = _GitUser.GetRepos();
		}
	}
}