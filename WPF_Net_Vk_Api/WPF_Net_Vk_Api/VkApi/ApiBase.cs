using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WPF_Net_Vk_Api.VkApi
{
    class ApiBase
    {
        private static long lastTick = DateTime.Now.Ticks;
        private static object _syncGet = new object();
        private static object _syncGetImage = new object();
        private static Dictionary<string, BitmapImage> imageDict = new Dictionary<string, BitmapImage>();
        protected static string Get(string method, string accessToken, string param)
        {
            lock (_syncGet)
            {
                while (DateTime.Now.Ticks - lastTick < 1.0 / 3)
                    Thread.Sleep(10);
                string url = "https://api.vk.com/method/" + method + "?" + param + String.Format("&v=5.31&access_token={0}", accessToken);
                WebRequest req = WebRequest.Create(url);
                WebResponse resp = req.GetResponse();
                Stream stream = resp.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string Out = sr.ReadToEnd();
                sr.Close();
                return Out;
            }
        }
        protected static string Execute(string accessToken, string code)
        {
            return Get("execute", accessToken, "code=" + code).Replace("\\/", "/");
        }
        public static BitmapImage GetImage(string imageSource)
        {
            if (!imageDict.ContainsKey(imageSource))
            {
                imageDict.Add(imageSource, null);

                System.Threading.ThreadPool.QueueUserWorkItem(
                    new System.Threading.WaitCallback(delegate(Object obj)
                        {
                            lock (_syncGetImage)
                            {
                                var c = new WebClient();
                                var bytes = c.DownloadData(imageSource);
                                var ms = new MemoryStream(bytes);
                                var bi = new BitmapImage();
                                bi.BeginInit();
                                bi.StreamSource = ms;
                                bi.EndInit();
                                imageDict[imageSource] = bi;
                            }
                        }));
            }
            return imageDict[imageSource];
        }
    }
}
