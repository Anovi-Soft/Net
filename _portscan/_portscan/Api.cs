using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _portscan
{
    class InvalidArgumentsException : System.Exception { }
    class Api
    {
        HashSet<String> help_msg = new HashSet<string>() { "/?", "-h", "--help", "help", "?" };
        HashSet<String> adress_msg = new HashSet<string>() { "-a", "--adress", "adress" };
        HashSet<String> tcp_msg = new HashSet<string>() { "-t", "--tcp", "tcp" };
        HashSet<String> udp_msg = new HashSet<string>() { "-u", "--udp", "udp" };
        HashSet<String> mthread_msg = new HashSet<string>() { "-m", "--mthread", "mthread" };
        HashSet<String> time_out_msg = new HashSet<string>() { "-o", "--timeout", "timeout" };
        public void start(String[] args)
        {
            if (args.Length == 0 || help_msg.Contains(args[0]))
                out_help();
            else
            {
                int i = 0;
                String adress = null;
                HashSet<int> tcp = null, udp = null;
                int time_out = 10000;
                int num_threads = 1;
                while (i < args.Length)
                {
                    if (adress_msg.Contains(args[i]))
                        adress = args[i + 1];
                    else if (tcp_msg.Contains(args[i]))
                        tcp = parse_range(args[i + 1]);
                    else if (udp_msg.Contains(args[i]))
                        udp = parse_range(args[i + 1]);
                    else if (time_out_msg.Contains(args[i]))
                    {
                        if (!int.TryParse(args[i + 1], out time_out))
                            throw new InvalidArgumentsException();
                    }
                    else if (mthread_msg.Contains(args[i]))
                    {
                        if (!int.TryParse(args[i + 1], out num_threads))
                            throw new InvalidArgumentsException();
                    }
                    else
                        throw new InvalidArgumentsException();
                    i += 2;
                }
                if (adress == null)
                    throw new InvalidArgumentsException();
                Console.WriteLine(String.Format("Сканирование портов у {0} со временем ожидания={1}", adress, time_out));
                if (tcp != null)
                {
                    Console.Write("TCP : ");
                    foreach (int value in tcp)
                        Console.Write(" "+value);
                    Console.WriteLine();
                }
                if (udp != null)
                {
                    Console.Write("UDP : ");
                    foreach (int value in udp)
                        Console.Write(" " + value);
                    Console.WriteLine();
                }
                if (udp == null && tcp == null)
                {
                    Console.WriteLine("Не указан не один порт.\nДля парсинга портов укажите хотябы один TCP или UDP порт.");
                }
                else
                {
                    Console.WriteLine();
                    PortScan scan = new PortScan(adress, 
                        tcp != null ? tcp.ToArray() : new int[] { },
                        udp != null ? udp.ToArray() : new int[] { },
                        time_out, 
                        num_threads);
                    scan.scaning();
                }
            }
        }
        private HashSet<int> parse_range(String range)
        {
            try
            {
                HashSet<int> ports = new HashSet<int>();
                foreach (String part in range.Replace(" ", "").Split(','))
                    if (part.Contains('-'))
                    {
                        var splt = part.Split('-');
                        if (splt.Length != 2)
                            throw new InvalidArgumentsException();
                        if (Convert.ToInt32(splt[0])>= Convert.ToInt32(splt[1]))
                            throw new InvalidArgumentsException();
                        for (int i = Convert.ToInt32(splt[0]); i < Convert.ToInt32(splt[1]); i++)
                            if (i < 1 || i > 65535) { throw new InvalidArgumentsException(); }
                            else ports.Add(i);
                    }
                    else
                        ports.Add(Convert.ToInt32(part));
                return ports;
            }
            catch (Exception)
            {
                throw new InvalidArgumentsException();
            }
        }
        public void out_help()
        {
            Console.WriteLine("Обязательный аргумент:\n  -a, --adress, adress : Адрес трассировки");
            Console.WriteLine("Необязательные аргументы:\n  -t, --tcp, tcp : Сканируемые TCP порты.");
            Console.WriteLine("  -u, --udp, udp : Сканируемые UDP порты.");
            Console.WriteLine("  -o, --timeout, timeout : Максимальное время ожинадиня в мс. (Стандартно:10000)");
        }
    }
}
