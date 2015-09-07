using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cons_Net_HTTP_Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press \"q\" if you want exit\n"+
                "Press \"s\" if you want start or stop listen");
            var proxy = HttpProxy.GetInstance(12388,
                new Loger(new List<TextWriter>{ 
                    Console.Out, 
                    new StreamWriter(DateTime.Now.ToString()
                        .Replace(':','_') + ".txt") }));
            proxy.Start();
            bool active = true;
            while (active) switch (Console.ReadKey(true).KeyChar)
            {
                case 'q':
                case 'й':
                    active = false;
                    break;
                case 's':
                case 'ы': 
                    if (proxy.Active) 
                        proxy.Stop();
                    else 
                        proxy.Start();
                    break;
                default: break;
            }
            proxy.Stop();
        }
    }
}
