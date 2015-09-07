using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Cons_Net_HTTP_Proxy
{
    class HttpProxy
    {
        //<singleton>
        private static HttpProxy instance;
        private static object _syncRoot = new object();
        public static HttpProxy GetInstance(int port, Loger loger = null)
        {
            if (HttpProxy.instance == null)
            {
                lock (_syncRoot)
                {
                    if (HttpProxy.instance == null)
                    {
                        HttpProxy.instance = new HttpProxy(port, loger);
                    }
                }
            }
            return HttpProxy.instance;
        }
        /// </singleton>
        /// <flags>
        private bool active;
        public bool Active
        {
            get { return active; }
        }
        /// <flags>
        /// <baseFields>
        int port;
        TcpListener listener;
        Loger loger;
        /// </baseFields>
        /// <constructors>
        private HttpProxy(int port, Loger loger)
        {
            this.port = port;
            this.loger = loger == null ? new Loger() : loger;
            loger.WriteLine("Hello, HttpProxy work in port:"+ port);
        }
        /// </constructors>
        /// <destructor>
        ~HttpProxy()
        {
            Stop();
            loger.WriteLine("HttpProxy GoodBye");
        }
        /// </destructor>
        /// <control>
        public void Start()
        {
            if (active)
                return;
            lock (this)
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                active = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(ListenerThread));
                loger.WriteLine("Start listen");
            }
        }

        public void Stop()
        {
            if (!active)
                return;
            lock (this)
            {
                active = false;
                listener.Stop();
                loger.WriteLine("End listen");
            }
        }
        /// </control>
        /// <threads>
        private void ListenerThread(Object arg)
        {
            lock (listener) while (active)
                if (listener.Pending())
                    ThreadPool.QueueUserWorkItem(
                        new WaitCallback(ClientThread),
                        listener.AcceptSocket());                
        }

        private void ClientThread(Object arg)
        {
            if (arg.GetType() != typeof(Socket))
                throw new ArgumentException();
            Socket sock = (Socket)arg;
            using (sock) try
            {
                byte[] request = readAll(sock);
                int port;
                String host = GetHost(request, out port);
                var entry = Dns.GetHostEntry(host);
                var ipAddres = entry.AddressList.Where(a => a.AddressFamily == AddressFamily.InterNetwork).First();
                var endPoint = new IPEndPoint(ipAddres, port);
                loger.WriteLine(String.Format("({0}) talk with ({1})", 
                    sock.RemoteEndPoint.ToString(),
                    endPoint.ToString()));
                using (Socket targetSocket = new Socket(
                    ipAddres.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp))
                {
                    targetSocket.Connect(endPoint);
                    if (targetSocket.Send(request, request.Length, SocketFlags.None) == request.Length)
                    {
                        byte[] respounce = readAll(targetSocket);
                        if (sock.Send(respounce, respounce.Length, SocketFlags.None) != respounce.Length)
                        {
                            loger.WriteLine(String.Format("Cant send respounce to {0}", sock.RemoteEndPoint.ToString()));
                        }
                    }
                    else
                    {
                        loger.WriteLine(String.Format("Cant send rquest to {0}", endPoint.ToString()));
                    }
                    targetSocket.Disconnect(false);
                }
            }
            catch (SocketException e) {}
            catch (Exception e)
                {
                    loger.WriteLine(e.ToString());
                }
        }
        /// <threads>
        /// <clientThreadHelper>
        private byte[] readAll(Socket sock)
        {
            byte[] buf = new byte[sock.ReceiveBufferSize];
            int len = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                while (sock.Poll(1000000, SelectMode.SelectRead) &&
                    (len=sock.Receive(buf, sock.ReceiveBufferSize, SocketFlags.None)) > 0)
                {
                    ms.Write(buf, 0, len);
                }
                return ms.ToArray();
            }
        }
        private string GetHost(byte[] request, out int port)
        {
            Regex reg = new Regex(@"Host: (((?<host>.+?):(?<port>\d+?))|(?<host>.+?))\s+", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            String line = Encoding.ASCII.GetString(request);
            Match m = reg.Match(line);
            if (!int.TryParse(m.Groups["port"].Value, out port)) 
                port = 80; 
            return m.Groups["host"].Value;
        }
        /// <\clientThreadHelper>
    }
}
