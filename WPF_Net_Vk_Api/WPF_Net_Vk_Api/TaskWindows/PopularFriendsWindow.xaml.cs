using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace WPF_Net_Vk_Api
{
    public partial class PopularFriendsWindow : Window
    {
        private VkApi.AsyncListOfPotencialFriends potencialFriends = new VkApi.AsyncListOfPotencialFriends();
        private PopularFriendsWindow(String accessToken) 
        {
           this.accessToken = accessToken;
           potencialFriends.Start(accessToken);
           InitializeComponent();
           new Thread(delegate()
               {
                   var persons = new List<VkApi.Person>();
                   while (potencialFriends.IsWork)
                   {
                       if (!potencialFriends.IsEmpty)
                       {
                           persons.AddRange(potencialFriends.LstItems);
                           persons.Sort(delegate(VkApi.Person a, VkApi.Person b) 
                           {
                               return -1 * a.mutual_friends.CompareTo(b.mutual_friends);
                           });
                           //try
                           //{
                           //    foreach (var person in persons)
                           //        person.Photo = VkApi.ApiBase.GetImage(person.photo_50);
                           //}
                           //catch (Exception e)
                           //    {
                           //        e.GetType();
                           //    }
                           Dispatcher.BeginInvoke(new ThreadStart(delegate()
                           {
                               listBox.Items.Clear();
                               foreach (var person in
                                    persons.Select(a => 
                                        new PersonInformationControl(a.Photo,
                                            a.first_name,
                                            a.last_name,
                                            a.mutual_friends,
                                            a.online,
                                            a.id)))
                               listBox.Items.Add(person);
                            }));
                       }
                       Thread.Sleep(100);
                   }
               }).Start();
        }
        private String accessToken;
        private static PopularFriendsWindow instance; 
        private static object _syncRoot = new object(); 
        public static PopularFriendsWindow GetInstance(String accessToken) 
        {  
            if (PopularFriendsWindow.instance == null) 
            { 
                lock (_syncRoot) 
                { 
                    if (PopularFriendsWindow.instance == null) 
                    { 
                        PopularFriendsWindow.instance = new PopularFriendsWindow(accessToken); 
                    } 
                } 
            }  
            return PopularFriendsWindow.instance; 
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            instance = null;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PFWindow.Width = 890;
            listBox.Height = PFWindow.Height - 55;
        }

        private void listBox_Initialized(object sender, EventArgs e)
        {
            listBox.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription("LMutualFriends",
                System.ComponentModel.ListSortDirection.Ascending));
        } 
    }
}
