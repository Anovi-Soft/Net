using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace _portscan
{
    class PortScan
    {
        Dictionary<int, string> tcp_ports_dict = new Dictionary<int, string>();
        Dictionary<int, string> udp_ports_dict = new Dictionary<int, string>();
        string _adress;
        int _timeOut;
        List<KeyValuePair<bool, int>>[] threads_args;//key: true = tcp; false = udp
        int _endThreads;
        int _threadCount;

        public PortScan(string adress, int[] tcp, int[] udp, int timeOut, int numThreads)
        {
            this._adress = adress;
            this._timeOut = timeOut;
            this.threads_args = CreateThreadArgs(tcp, udp, numThreads);
            load_dicts();
            foreach (var tmp in threads_args)
                _threadCount += tmp.Count;
        }
        private void load_dicts(string addres= @"c:\Windows\System32\drivers\etc\services")
        {
            if (!File.Exists(addres))
                return;
            var file = File.OpenText(addres);
            var str = "";
            while ((str = file.ReadLine()) != null)
            {
                if (str.StartsWith("#") || str.Length == 0) continue;
                var splt = Regex.Replace(str, @"\s+", " ").Split(' ');
                var splt2 = splt[1].Split('/');
                if (splt2[1].Equals("tcp"))
                    tcp_ports_dict.Add(Convert.ToInt32(splt2[0]), splt[0]);
                else
                    udp_ports_dict.Add(Convert.ToInt32(splt2[0]), splt[0]);
            }
        }

        public static List<KeyValuePair<bool, int>>[] CreateThreadArgs(int[] tcp, int[] udp, int numThreads)
        {
            var result = new List<KeyValuePair<bool, int>>[numThreads];
            for (var i = 0; i < numThreads; i++)
                result[i] = new List<KeyValuePair<bool, int>>();
            var threadId = 0;
            foreach (var b in new[] {false,true})
                foreach (var port in b ? tcp : udp)
                {
                    result[threadId].Add(new KeyValuePair<bool, int>(b, port));
                    threadId = (threadId + 1) % numThreads;
                }
            return result;

        }
        public void scaning()
        {
            foreach(var args in threads_args)
            {
                var thread = new Thread(() => {
                    foreach (var arg in args)
                    {
                        if (arg.Key)
                            work_tcp(arg.Value);
                        else
                            work_udp(arg.Value);
                        this._endThreads++;
                    }
                });
                thread.Start();
            }
            while (true)
                if (_endThreads < _threadCount)
                    Thread.Sleep(100);
                else
                    break;
            Console.WriteLine("-----------------------------\nСканирование портов завершено\n-----------------------------");
        }
        private void work_tcp(int port)
        {
            var client = new TcpClient();
            var ar = client.BeginConnect(_adress, port, null, null);
            var success = ar.AsyncWaitHandle.WaitOne(_timeOut, false);

            if (!success || !client.Connected)
                Console.Out.WriteLine("-TCP {0}({1} port) is close", tcp_ports_dict.ContainsKey(port) ? tcp_ports_dict[port] : "", port);
            else
                Console.Out.WriteLine("-TCP {0}({1} port) is open", tcp_ports_dict.ContainsKey(port) ? tcp_ports_dict[port] : "", port);
        }
        private void work_udp(int port)
        {
            var client = new UdpClient(port) {Client = {ReceiveTimeout = _timeOut } };
            client.Connect(this._adress, port);
            var msg = Encoding.ASCII.GetBytes("lollololol");
            client.Send(msg, msg.Length);
            var adress = Dns.GetHostAddresses(this._adress)[0].Address;
            var ip_end = new IPEndPoint(adress, port);
            try
            {
                var answer = client.Receive(ref ip_end);
                if (answer.Length != 0)
                    Console.Out.WriteLine("-UDP {0}({1} port) is open", this.udp_ports_dict.ContainsKey(port) ? udp_ports_dict[port] : "", port);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10054)
                    Console.Out.WriteLine("-UDP {0}({1} port) is close", this.udp_ports_dict.ContainsKey(port) ? udp_ports_dict[port] : "", port);
                if (e.ErrorCode == 10060)
                    Console.Out.WriteLine("-UDP {0}({1} port) is open", this.udp_ports_dict.ContainsKey(port) ? udp_ports_dict[port] : "", port);
            }
        }
    }

}
