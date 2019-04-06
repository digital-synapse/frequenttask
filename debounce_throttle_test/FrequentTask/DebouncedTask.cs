using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace debounce_throttle_test.FrequentTask
{



    public abstract class DebouncedTask : IFrequentTask
    {
        /// <summary>
        /// Creates an executable async task that can be not be executed more than once at the same time. optionally frequency limit can be specified
        /// </summary>
        /// <param name="frequencyLimitMillis">optional frequency limit to be specified</param>
        public DebouncedTask(long frequencyLimitMillis=0)
        {
            this.frequencyLimitMillis = frequencyLimitMillis;
            cts = new CancellationTokenSource();
            tcs = new TaskCompletionSource<bool>();
            stopwatch = new Stopwatch();
        }

        public abstract Task ExecutableTask(CancellationToken token);

        private Stopwatch stopwatch;
        private long frequencyLimitMillis;
        private Task runningTask;
        private CancellationTokenSource cts;
        private TaskCompletionSource<bool> tcs;
        private bool manualCancel = false;

        /// <summary>
        /// Cancel execution of the currently running task and reject any future invocations
        /// </summary>
        public void Cancel()
        {
            manualCancel = true;
            cts.Cancel();
        }

        /// <summary>
        /// Execute the task, unless it is currently running or the required time has not elapsed
        /// </summary>
        /// <returns>true if the task ran to completion, otherwise false</returns>
        public async Task<FrequentTaskExecutionResult> Execute()
        {
            if (manualCancel) return FrequentTaskExecutionResult.ExecutionDebouncedPreviouslyCancelled;

            FrequentTaskExecutionResult result = FrequentTaskExecutionResult.Unknown;            
            try
            {                
                if (runningTask == null)
                {
                    if (frequencyLimitMillis != 0) stopwatch.Start();
                    runningTask = ExecutableTask(cts.Token);
                    await runningTask;
                    result = FrequentTaskExecutionResult.ExecutionCompleted;
                }
                else
                {
                    if (runningTask.IsCompleted && (frequencyLimitMillis != 0 && stopwatch.ElapsedMilliseconds > frequencyLimitMillis))
                    {
                        stopwatch.Restart();
                        //cts.Cancel();
                        cts = new CancellationTokenSource();
                        runningTask = ExecutableTask(cts.Token);
                        await runningTask;
                        if (runningTask.IsCompleted && (frequencyLimitMillis != 0 && stopwatch.ElapsedMilliseconds > frequencyLimitMillis * 2))
                        {
                            result = FrequentTaskExecutionResult.ExecutionFinishedDebounceTaskRanToCompletion;
                            tcs.TrySetResult(true);
                        }
                        else result = FrequentTaskExecutionResult.ExecutionCompletedTaskInvokedAgain;
                    }
                    else
                    {
                        result = FrequentTaskExecutionResult.ExecutionDebouncedTaskNotInvoked;
                    }
                }
                
            }
            catch (OperationCanceledException ex)
            {
                if (manualCancel)
                    result = FrequentTaskExecutionResult.ExecutionDebouncedCancelledManually;
                else
                    result = FrequentTaskExecutionResult.ExecutionCompletedTaskInvokedAgain;
            }

            
            return result;
        }

        public Task Completion => tcs.Task;
    }
}
