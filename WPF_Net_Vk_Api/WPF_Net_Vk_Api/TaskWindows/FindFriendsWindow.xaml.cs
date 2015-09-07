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
using System.Windows.Shapes;

namespace WPF_Net_Vk_Api
{
    public partial class FindFriendsWindow : Window
    {
        private FindFriendsWindow(String accessToken) 
        {
            this.accessToken = accessToken;
            InitializeComponent();
            
        }
        private String accessToken;
        private static FindFriendsWindow instance; 
        private static object _syncRoot = new object();
        public static FindFriendsWindow GetInstance(String accessToken) 
        {
            if (FindFriendsWindow.instance == null) 
            { 
                lock (_syncRoot) 
                {
                    if (FindFriendsWindow.instance == null) 
                    {
                        FindFriendsWindow.instance = new FindFriendsWindow(accessToken); 
                    } 
                } 
            }
            return FindFriendsWindow.instance; 
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            instance = null;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FFWindow.Width = 828;
            lisBox.Height = FFWindow.Height - 51;
        } 

    }
}
