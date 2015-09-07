using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _tracert
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new Api();
            try
            {
                api.start("-a urfu.ru".Split(' '));
            }
            catch (InvalidArgumentsException)
            {
                Console.WriteLine("Не верный список параметров");
                api.out_help();
            }
            catch (Exception)
            {
                Console.WriteLine("Неизвестная ошибка");
            }
            Console.ReadKey();
        }
    }
}
