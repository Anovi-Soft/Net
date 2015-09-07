using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _tracert
{
    class InvalidArgumentsException : System.Exception { }
    class Api
    {
        HashSet<String> help_msg = new HashSet<string>() { "/?", "-h", "--help", "help", "?" };
        HashSet<String> adress_msg = new HashSet<string>() { "-a", "--adress", "adress" };
        HashSet<String> ttl_msg = new HashSet<string>() { "-t", "--ttl", "ttl" };
        HashSet<String> time_out_msg = new HashSet<string>() { "-o", "--timeout", "timeout" };
        public void start(String []args)
        {
            if (args.Length == 0 || help_msg.Contains(args[0]))
                out_help();
            else
            {
                int i = 0;
                String adress = null;
                int ttl = 30;
                int time_out = 10000;
                while (i<args.Length)
                {
                    if (adress_msg.Contains(args[i]))
                        adress = args[i + 1];
                    if (ttl_msg.Contains(args[i]))
                        if (!int.TryParse(args[i + 1], out ttl))
                            throw new InvalidArgumentsException();
                    if (ttl_msg.Contains(args[i]))
                        if (!int.TryParse(args[i + 1], out time_out))
                            throw new InvalidArgumentsException();
                    i += 2;
                }
                if (adress == null)
                    throw new InvalidArgumentsException();
                Console.WriteLine(String.Format("Трассировка {0} с максимальным ttl={1} и временем ожидания={2}\n", adress, ttl, time_out));
                tracert(adress, ttl, time_out);
            }
        }
        private void tracert(String adress, int max_ttl, int time_out)
        {
            var trcrt = new Tracert(adress, max_ttl, time_out);
            Console.WriteLine("STL\tTime\tAdress\t\tInformation\n");
            try
            {
                var answer = trcrt.trace();
                while (answer != null)
                {
                    Console.WriteLine(answer);
                    answer = trcrt.trace();
                }
                Console.WriteLine("\n\nТрассировка Завершена");
            }
            catch (System.Net.NetworkInformation.PingException)
            {
                Console.WriteLine(String.Format("Не удается подключиться к {0}", adress));
            }
            catch (System.Exception)
            {
                Console.WriteLine("Неизвестная ошибка");
            }
            Console.ReadKey();
        }
        public void out_help()
        {
            Console.WriteLine("Обязательный аргумент:\n  -a, --adress, adress : Адрес трассировки");
            Console.WriteLine("Необязательные аргументы:\n  -t, --ttl, ttl : Максимальный ttl. (Стандартно:30)");
            Console.WriteLine("  -o, --timeout, timeout : Максимальное время ожинадиня в мс. (Стандартно:10000)");
        }
    }
}
