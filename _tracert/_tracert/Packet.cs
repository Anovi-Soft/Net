using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _tracert
{
    class Stock
    {
        public static Packet ECHO()
        {

            return new Packet(new List<KeyValuePair<string, int>>() {
            new KeyValuePair<string,int>("Type",8),
            new KeyValuePair<string,int>("Code",8),
            new KeyValuePair<string,int>("Checksum",16),
            new KeyValuePair<string,int>("Identifier",16),
            new KeyValuePair<string,int>("Sequence Number",16),
            new KeyValuePair<string,int>("Data",32)
            });
        }
        public static Packet SNTP()
        {
            return new Packet(new List<KeyValuePair<string, int>>() {
            new KeyValuePair<string,int>("LI", 2),
            new KeyValuePair<string,int>("VN", 3),
            new KeyValuePair<string,int>("Mode", 3),
            new KeyValuePair<string,int>("Stratum", 8),
            new KeyValuePair<string,int>("Poll", 8),
            new KeyValuePair<string,int>("Precision", 8),
            new KeyValuePair<string,int>("Root Delay", 32),
            new KeyValuePair<string,int>("Root Dispersion", 32),
            new KeyValuePair<string,int>("Reference Identifier", 32),
            new KeyValuePair<string,int>("Reference Timestamp", 64),
            new KeyValuePair<string,int>("Originate Timestamp", 64),
            new KeyValuePair<string,int>("Receive Timestamp", 64),
            new KeyValuePair<string,int>("Transmit Timestamp", 64)
        }, "SNTP");
        }
    }
    class Packet
    {
        byte[] bytes;
        string name;
        Dictionary<string, KeyValuePair<int, int>> grid = new Dictionary<string, KeyValuePair<int, int>>();
        public Packet(List<KeyValuePair<string, int>> prot_grid,
            string name = "UNKNOWN")
        {
            this.name = name;
            int i = 0;
            foreach (var prot in prot_grid)
            {
                grid.Add(prot.Key, new KeyValuePair<int, int>(i, prot.Value));
                i += prot.Value;
            }
            this.bytes = new byte[i / 8];
        }
        public void set(String key, ulong value)
        {
            if (!grid.ContainsKey(key))
                throw new IndexOutOfRangeException();
            byte[] split = BitConverter.GetBytes(value);
            if (value < 256)
            {
                int index = grid[key].Key / 8;
                int length = grid[key].Key % 8;
                set_bits(index, length, grid[key].Value, Convert.ToByte(value));
            }
            else
            {
                var nulls = grid[key].Value / 8 - split.Length;
                for (int i = 0; i < nulls; i++)
                    bytes[grid[key].Key / 8 + i] = 0;
                for (int i = nulls; i < grid[key].Value / 8; i++)
                    bytes[grid[key].Key / 8 + i] = split[i - nulls];
            }
        }
        private void set_bits(int index, int shift, int size, byte value)
        {
            var b1 = value << (8 - size - shift);
            var b2 = (int)(Math.Pow(2, size) - 1) << (8 - size - shift);
            b1 &= b2;
            bytes[index] &= (byte)~b2;
            bytes[index] |= (byte)b1;
        }
        public byte[] get_array()
        {
            return this.bytes;
        }
        public void set_array(byte[] array)
        {
            if (this.bytes.Length != array.Length)
                throw new Exception("Неправильный размер пакета");
            this.bytes = array;
        }
        public string ToString()
        {
            string result = name + "\n";
            for (int i = 0; i < bytes.Length; i++)
            {
                result += Convert.ToString(bytes[i], 2).PadLeft(8, '0') + " ";
                if (i % 4 == 3) { result += "\n"; }
            }
            return result;
        }
    }

}
