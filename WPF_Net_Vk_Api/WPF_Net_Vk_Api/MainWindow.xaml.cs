using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WPF_Net_Vk_Api
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OAuth oauth;
        private String accessToken;
        public MainWindow()
        {
            InitializeComponent();
            oauth = new OAuth(4753921, 2, OAuthOk, OAuthError);
            oauth.Show();
            this.Hide();
        }
        private void OAuthOk(object sender, String e)
        {
            accessToken = e;
            oauth.Close();
            this.Show();
        }
        private void OAuthError(object sender, String e)
        {
            oauth.Close();
            this.Close();
        }

        private void Button_Click(object sender = null, RoutedEventArgs e = null)
        {
            var popularFiendsWindow = PopularFriendsWindow.GetInstance(accessToken);
            popularFiendsWindow.Show();
            popularFiendsWindow.Focus();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }
    }
}
