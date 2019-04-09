using debounce_throttle_test.FrequentTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace debounce_throttle_test
{
    public class TestThrottledTask : ThrottledTask
    {
        public TestThrottledTask() : base(500) {}

        public override async Task ExecutableTask(CancellationToken token)
        {
            ConsoleAsync.WriteAt("Throttle                                                                                                     ", 0, 2);
            for (var i = 0; i < 50; i++)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                else
                {
                    if (i == 9)
                        ConsoleAsync.WriteAt("|-", 9 + i, 2);
                    else
                        ConsoleAsync.WriteAt("->", 9 + i, 2);

                    await Task.Delay(50);

                }
            }
        }
    }
}
