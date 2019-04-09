using debounce_throttle_test.FrequentTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace debounce_throttle_test
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Write("Press any key to trigger, Escape to quit...");

            var tTest = new TestThrottledTask();
            var dTest = new TestDebouncedTask();

            ConsoleKeyInfo key = new ConsoleKeyInfo();
            while (key.Key != ConsoleKey.Escape)
            {
                var bgtask1 = tTest.Execute().ContinueWith(t => ConsoleAsync.WriteAt(t.Result.ToString(),61, 2) );
                var bgtask2= dTest.Execute().ContinueWith(t => ConsoleAsync.WriteAt(t.Result.ToString(), 61, 3) );
                key = Console.ReadKey();
            }
        }        
    }
}

