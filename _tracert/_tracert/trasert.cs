using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net;

namespace _tracert
{
    class Tracert
    {
        private int max_ttl;
        private int time_out;
        private byte[] buffer;
        private string end_adress;
        private int step_ttl;
        private Dictionary<int, string> ip_dict;
        private Dictionary<string, string> country_dict;
        public Tracert(String adress, int max_ttl = 30, int time_out = 10000)
        {
            this.max_ttl = max_ttl;
            this.time_out = time_out;
            const string msg = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            this.buffer = Encoding.ASCII.GetBytes(msg);
            this.end_adress = adress;
            this.step_ttl = 0;
            this.ip_dict = get_whois_dict("ip_whois.txt");
            this.country_dict = get_country_dict("country.txt");
        }

        private Dictionary<string, string> get_country_dict(string file_name)
        {
            var result = new Dictionary<string, string>();

            var file = File.OpenText(file_name);
            string str = "";
            while ((str = file.ReadLine()) != null)
            {
                var splt = str.Split('	');
                result.Add(splt[0].ToLower(), splt[1]);
            }
            return result;
        }
        private string get_info(string adress)
        {
            string server = null;
            int first = Convert.ToInt32(adress.Split('.').First());
            if ((first == 10)||(first == 192 && adress.Split('.')[1] == "168"))
                return "Это локальный адрес";

            if (ip_dict.ContainsKey(first))
                server = ip_dict[first];
            else
                return "Нет подходящего whois сервера";

            string country = "UNKNOWN";
            string strResponse = "";
            TcpClient tcpWhois = new TcpClient(server, 43);
            NetworkStream nsWhois = tcpWhois.GetStream();
            BufferedStream bfWhois = new BufferedStream(nsWhois);
            StreamWriter swSend = new StreamWriter(bfWhois);
            swSend.WriteLine(adress);
            swSend.Flush();
            StreamReader srReceive = new StreamReader(bfWhois);
            strResponse = srReceive.ReadLine();
            while (strResponse != null)
            {
                strResponse = strResponse.ToLower();
                if (strResponse.StartsWith("country"))
                {
                    country = strResponse.Substring(strResponse.Length - 2);
                    break;
                }
                strResponse = srReceive.ReadLine();
            }
            tcpWhois.Close();
            if (this.country_dict.ContainsKey(country))
                country = this.country_dict[country];
            else
                country.ToUpper();
            return country;
        }
        private long get_adress(string adress)
        {
                return Dns.GetHostAddresses(adress)[0].Address;
        }
        private Dictionary<int,string> get_whois_dict(string file_name)
        {
            var result = new Dictionary<int, string>();
            
            var file = File.OpenText(file_name);
            string str = "";
            while ((str = file.ReadLine()) != null)
            {
                var splt = str.Split(' ');
                result.Add(Convert.ToInt32(splt[0]), splt[1]);
            }
            return result;
        }
        private string _to_string(PingReply reply)
        {
            switch (reply.Status)
            {
                case IPStatus.Success:
                case IPStatus.TtlExpired:
                    var time = reply.RoundtripTime;
                    var adress = reply.Address.ToString();
                    return String.Format("{0}\t{1}\t{2}\t{3}",
                        step_ttl,
                        time,
                        adress,
                        get_info(adress));
                case IPStatus.TimedOut:
                    return String.Format("{0}\t*\t*.*.*.*\t\t{1}",
                        step_ttl,
                        "Превышен интервал ожидания для запроса");
                default:
                    return String.Format("NonParse IPStatus: {0}", reply.Status.ToString());
            }
        }
        public string trace()
        {
            step_ttl++;
            if (end_adress == "" || this.step_ttl > max_ttl)
                return null;
            if (this.step_ttl == max_ttl)
                return "Превышен TTL";
            var reply = trace_rt(end_adress, step_ttl);
            if (reply.Status == IPStatus.Success)
            {
                end_adress = "";
            }
            else if (reply.Address != null)
            {
                reply = trace_rt(reply.Address.ToString(), step_ttl);
            }
            return _to_string(reply);

        }
        protected PingReply trace_rt(String adress, int ttl)
        {
            var pinger = new Ping();
            var options = new PingOptions(ttl, true);
            return pinger.Send(adress, time_out, buffer, options);
        }
        protected PingReply my_trace_rt(String adress, int ttl)
        {
            var socket = new Socket(SocketType.Raw, ProtocolType.IPv4);
            socket.Ttl = Convert.ToInt16(ttl);

            Packet packet = Stock.ECHO();
            packet.set("Type", 8);
            EndPoint end_point = new IPEndPoint(new IPAddress(get_adress(adress)), 0);
            socket.SendTo(packet.get_array(), end_point);
            byte[] answer = packet.get_array();
            socket.ReceiveFrom(answer, ref end_point);
            //var tmp = new Socket(SocketType.Raw,ProtocolType.Icmp);
            //tmp.Send()
            return null;
        }
    }
}
