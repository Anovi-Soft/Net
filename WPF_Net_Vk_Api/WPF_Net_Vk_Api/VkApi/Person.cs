using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WPF_Net_Vk_Api.VkApi
{
    class Person
    {
        public string id{get; set;}
        public string first_name{get; set;}
        public string last_name{get; set;}
        public string photo_50 { get; set; }
        public BitmapImage Photo = null;
        private string _online;
        public string online {
            get { return _online.Equals("0") ? "Offline" : "Online"; }
            set { _online = value; } 
        }
        public int mutual_friends { get; set; }
    }
}
