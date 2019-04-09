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
        public TestDebouncedTask() : base(500) {}

        public override async Task ExecutableTask(CancellationToken token)
        {
            ConsoleAsync.WriteAt("Debounce                                                                                                     ", 0, 3);
            for (var i = 0; i < 50; i++)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                else
                {
                    ConsoleAsync.WriteAt("->", 9 + i, 3);
                    await Task.Delay(50);
                }
            }
        }
    }
}
