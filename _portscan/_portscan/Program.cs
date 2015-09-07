using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _portscan
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new Api();
            try
            {
                api.start(args);
            }
            catch (InvalidArgumentsException)
            {
                Console.WriteLine("Неправильные аргументы");
                api.out_help();
            }/*
            catch (Exception)
            {
                Console.WriteLine("Неизвестная ошибка");
            }*/
                Console.ReadKey();
        }
    }
}
