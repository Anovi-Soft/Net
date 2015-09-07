using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_Net_Vk_Api
{
    /// <summary>
    /// Логика взаимодействия для OAuth.xaml
    /// </summary>
    public partial class OAuth : Window
    {
        public delegate void AccessTokenEventHandler(object sender, String e);
        public event AccessTokenEventHandler AccessTokenGiven;
        public event AccessTokenEventHandler AccessDenied;
        public OAuth(int appId,
            int scope,
            AccessTokenEventHandler accessTokenGivenEvent,
            AccessTokenEventHandler accessDeniedEvent)
        {
            InitializeComponent();
            AccessTokenGiven = accessTokenGivenEvent;
            AccessDenied = accessDeniedEvent;
            string url = String.Format("https://oauth.vk.com/authorize?client_id={0}&scope={1}&redirect_uri=https://oauth.vk.com/blank.html&display=popup&v=5.28&response_type=token&revoke=0", appId, scope);
            webBrowser.Navigate(url);
        }

        private void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Uri.ToString().IndexOf("access_token") != -1 || 
                e.Uri.ToString().IndexOf("error_description") != -1 )
            {
                Regex myReg = new Regex(@"(?<name>[\w\d\x5f]+)=(?<value>[^\x26\t\n\r\f]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match m in myReg.Matches(e.Uri.ToString()))
                {
                    if (m.Groups["name"].Value == "access_token")
                    {
                        AccessTokenGiven(this, m.Groups["value"].Value);
                        break;
                    }
                    if (m.Groups["name"].Value == "error_description")
                    {
                        AccessDenied(this, m.Groups["value"].Value);
                        break;
                    }
                }
            }
        }
        
    }
}
