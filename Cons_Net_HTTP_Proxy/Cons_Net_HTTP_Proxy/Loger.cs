using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cons_Net_HTTP_Proxy
{
    class Loger
    {
        List<TextWriter> writers;
        public Loger(List<TextWriter> writers)
        {
            this.writers = writers;
        }
        public Loger(TextWriter writer)
        {
            this.writers = new List<TextWriter>(){ writer };
        }
        public Loger()
        {
            this.writers = new List<TextWriter>();
        }
        public void Write(String text)
        {
            foreach (var writer in writers)
            {
                writer.Write(String.Format("[{0}] - {1}", DateTime.Now.ToString(), text));
                writer.Flush();
            }
        }
        public void WriteLine(String text)
        {
            Write(text + "\r\n");
        }
    }
}
