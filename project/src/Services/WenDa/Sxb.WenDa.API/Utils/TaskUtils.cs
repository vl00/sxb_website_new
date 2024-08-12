
namespace Sxb.WenDa.API.Utils
{
    internal static class TaskUtils
    {
        /// <summary>
        /// await $task.AwaitNoErr(); // no-throw
        /// </summary>
        public static Task AwaitNoErr(this Task task)
        {
            if (task?.IsCompletedSuccessfully != false) return task;
            return task.ContinueWith(static t => _ = t.Exception,
                CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        #region AwaitResOrErr
        /// <summary>
        /// var (_, ex) = await $task.AwaitCaptureResult(); // no-throw
        /// </summary>
        public static Task<(object Result, Exception Error)> AwaitResOrErr(this Task task)
        {
            return task.ContinueWith<(object, Exception)>(static t => (null, (t.Exception?.InnerExceptions?.FirstOrDefault() ?? t.Exception)),
                CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
        /// <summary>
        /// var (r, ex) = await $task.AwaitResOrErr(); // no-throw
        /// </summary>
        public static Task<(T Result, Exception Error)> AwaitResOrErr<T>(this Task<T> task)
        {
            return task.ContinueWith(static t => ((t.Exception == null ? t.Result : default), (t.Exception?.InnerExceptions?.FirstOrDefault() ?? t.Exception)),
                CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
        #endregion AwaitResOrErr
    }
}
