using debounce_throttle_test.FrequentTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace debounce_throttle_test
{
    public class TestDebouncedTask : DebouncedTask
    {
        public TestDebouncedTask() : base(250) {}

        public override async Task ExecutableTask(CancellationToken token)
        {            
            for (var i = 0; i < 10; i++)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                await Task.Delay(50);
                Console.Write(i + " ");
            }
        }
    }
}
