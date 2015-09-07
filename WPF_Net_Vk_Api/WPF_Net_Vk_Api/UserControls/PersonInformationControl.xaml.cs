using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
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
    /// Логика взаимодействия для PersonInformationControl.xaml
    /// </summary>
    public partial class PersonInformationControl : UserControl
    {
        private String link;
        public int MutualFriends;
        public PersonInformationControl(BitmapImage imageSource, String firstName, String lastName, int mutualFriends, String online, String link = null)
        {
            InitializeComponent();
            LName.Content = String.Format("{1} {0}", firstName, lastName);
            LMutualFriends.Content = MutualFriends = mutualFriends;
            LOnline.Content = online;
            this.link = link;
            IAvatar.Source = imageSource;
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(String.Format("http://vk.com/id{0}", link));
        }
        //public static void ImageLoad(PopularFriendsWindow pfw)
        //{
        //    ImageLoadEnd = false;
        //    while (!ImageLoadEnd || queueImageLoad.Count != 0)
        //    {
        //        if (queueImageLoad.Count != 0)
        //        {
        //            var step = queueImageLoad.Dequeue();
        //            pfw.Dispatcher.BeginInvoke(new ThreadStart(delegate() { step.Value.Source = VkApi.ApiBase.GetImage(step.Key); }));
        //        }
        //    }
        //}
    }
}
