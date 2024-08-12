namespace Sxb.WenDa.API.BackgroundServices
{
    public abstract class BaseBackgroundService : BackgroundService
    {
        public TimeSpan Delay { get; set; }

        protected BaseBackgroundService(TimeSpan delay)
        {
            Delay = delay;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ExecutingAsync(stoppingToken);
                await Task.Delay(Delay);//一个小时
            }
        }

        protected abstract Task ExecutingAsync(CancellationToken stoppingToken);
    }
}
