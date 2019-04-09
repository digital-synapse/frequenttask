using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace debounce_throttle_test
{
    public class ConsoleAsync
    {

        private static object locker = new object();

        public static void WriteAt(string s, int x, int y)
        {
            lock (locker)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(s);
            }
        }
    }
}
