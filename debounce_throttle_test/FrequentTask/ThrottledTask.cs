using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace debounce_throttle_test.FrequentTask
{
    public abstract class ThrottledTask : IFrequentTask
    {
        /// <summary>
        /// Creates an executable async task that can be interrupted / restarted by subsequent invocations
        /// </summary>
        /// <param name="frequencyLimitMillis">Optionally allows a frequency limit to be specified</param>
        public ThrottledTask(long frequencyLimitMillis=0)
        {
            this.frequencyLimitMillis = frequencyLimitMillis;
            cts = new CancellationTokenSource();
            tcs = new TaskCompletionSource<bool>();
            stopwatch = new Stopwatch();
            manualCancel = false;
        }

        public abstract Task ExecutableTask(CancellationToken token);

        private readonly Stopwatch stopwatch;
        private readonly long frequencyLimitMillis;
        private readonly TaskCompletionSource<bool> tcs;

        private Task runningTask;
        private CancellationTokenSource cts;        
        private bool manualCancel;

        /// <summary>
        /// Cancel execution of the currently running task and reject any future invocations
        /// </summary>
        public void Cancel()
        {
            manualCancel = true;
            cts.Cancel();
        }

        /// <summary>
        /// Execute the task, interrupting the currently running task if needed. 
        /// If the task is already running, this will send a cancellation request and start a new task
        /// </summary>
        /// <returns>true if the task ran to completion, otherwise false</returns>
        public async Task<FrequentTaskExecutionResult> Execute()
        {
            if (manualCancel) return FrequentTaskExecutionResult.ExecutionThrottledPreviouslyCancelled;

            FrequentTaskExecutionResult result = FrequentTaskExecutionResult.ExecutionFinishedThrottledTaskRanToCompletion;            
            try
            {                
                if (runningTask == null)
                {
                    if (frequencyLimitMillis != 0) stopwatch.Start();
                    runningTask = ExecutableTask(cts.Token);
                    await runningTask;
                }
                else
                {
                    if (frequencyLimitMillis != 0 && stopwatch.ElapsedMilliseconds > frequencyLimitMillis)
                    {
                        stopwatch.Restart();
                        cts.Cancel();
                        cts = new CancellationTokenSource();
                        runningTask = ExecutableTask(cts.Token);
                        await runningTask;
                    }
                    else
                    {
                        result = FrequentTaskExecutionResult.ExecutionThrottledTaskNotStarted;
                    }
                }
                
            }
            catch (OperationCanceledException ex)
            {
                if (manualCancel)
                    result = FrequentTaskExecutionResult.ExecutionThrottledCancelledManually;
                else
                    result = FrequentTaskExecutionResult.ExecutionInterruptedTaskRestarted;
            }

            if (result == FrequentTaskExecutionResult.ExecutionFinishedThrottledTaskRanToCompletion) tcs.TrySetResult(true);             
            return result;
        }

        public Task Completion => tcs.Task;
    }
}
