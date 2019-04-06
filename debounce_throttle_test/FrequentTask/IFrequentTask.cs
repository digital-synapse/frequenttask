using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace debounce_throttle_test.FrequentTask
{
    public interface IFrequentTask
    {
        Task<FrequentTaskExecutionResult> Execute();
        void Cancel();
        Task Completion { get; }
    }

    public enum FrequentTaskExecutionResult
    {
        Unknown = 0,

        // debounce
        ExecutionFinishedDebounceTaskRanToCompletion = 1,
        ExecutionCompletedTaskInvokedAgain = 2,
        ExecutionDebouncedTaskNotInvoked = 3,
        ExecutionDebouncedCancelledManually = 4,
        ExecutionDebouncedPreviouslyCancelled = 5,
        ExecutionCompleted = 6,
    
        // throttle
        ExecutionFinishedThrottledTaskRanToCompletion = 107,
        ExecutionInterruptedTaskRestarted = 108,
        ExecutionThrottledTaskNotStarted = 109,
        ExecutionThrottledCancelledManually = 110,
        ExecutionThrottledPreviouslyCancelled = 111
    }
}
