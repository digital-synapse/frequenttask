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
            runTest("Throttled Task Test", new TestThrottledTask())
                .ContinueWith(x=> runTest("Debounced Task Test", new TestDebouncedTask())
                .ContinueWith(t => {
                     Console.WriteLine();
                     Console.WriteLine("Press any key...");                     
                 }));

            Console.ReadKey(); // block UI thread
        }

        private static async Task runTest(string description, IFrequentTask frequentTask)
        {
            Console.WriteLine("\n"+description);
            Console.WriteLine("--------------------------------------------");
            for (var i = 0; i < 5; i++)
            {
                await Task.Delay(100 * i);
                var bg = frequentTask.Execute().ContinueWith(t => { Console.WriteLine(t.Result); });
            }
            await frequentTask.Completion;
        }        
    }
}

