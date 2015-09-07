/*using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("tyz 210 - Admin tools");
            String host = System.Net.Dns.GetHostName();
            Console.WriteLine();
            //user name
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            Console.Write(wi.Name + "\n");
            Console.WriteLine();
            // host name
            Console.WriteLine("host: " + host);
            long cip = System.Net.Dns.GetHostByName(host).AddressList.Length - 1;
            for (int i = 0; i < cip; i++)
            {
                System.Net.IPAddress ip = System.Net.Dns.GetHostByName(host).AddressList[i];
                String lin = "  ip: " + ip.ToString();
                Console.WriteLine(lin);
                //show more info
                int lim = 9;
                String tab = "";
                for (int j = 0; j < lim; j++) tab += " ";
                String inf = System.Net.Dns.GetHostByName(host).AddressList[i].Address.ToString();
                Console.WriteLine(tab + ">Address: " + inf);
            }
            Console.WriteLine();
            Console.WriteLine("Get remote ip");
            String cmd = "n";
            String hst;
            while (cmd == "n")
            {
                Console.WriteLine("type host");
                hst = Console.ReadLine();
                System.Net.IPAddress[] rip;
                try
                {
                    rip = System.Net.Dns.GetHostByName(hst).AddressList;
                    for (int i = 0; i < rip.GetLength(0); i++)
                        Console.WriteLine("   >ip: " + rip[i].ToString());
                }
                catch
                {
                    Console.WriteLine("Error. Pleace try again");
                }
                Console.WriteLine();
                Console.WriteLine("Exit (y/n)");
                cmd = Console.ReadLine();
                Console.WriteLine();
            }

        }
    }*/