using MediatR;
using Sxb.Framework.AspNetCoreHelper.Utils;

namespace Sxb.WenDa.API.Utils
{
    public class AsyncUtils
    {        
        public static IServiceScopeFactory ServiceScopeFactory => HttpContextModel.GetServiceScopeFactory();

        public static event Action<IServiceProvider, Exception> OnBackgroundError;

        /// <summary>
        /// Safely do async operation in asp.net core DI
        /// </summary>
        /// <param name="func"></param>
        /// <param name="state"></param>
        public static async void StartNew(Func<IServiceProvider, object, Task> func, object state = null)
        {            
            if (func == null) throw new ArgumentNullException(nameof(func));
            await Task.Delay(100).ConfigureAwait(false);
            using var scope = ServiceScopeFactory.CreateScope();
            try 
            { 
                await func(scope.ServiceProvider, state).ConfigureAwait(false); 
            }
            catch (Exception ex)
            {
                var ev = OnBackgroundError;
                if (ev == null) return;
                lock (ServiceScopeFactory)
                {
                    ev(scope.ServiceProvider, ex);
                }
            }
        }

        /// <summary>
        /// Safely do async operation with MediatR in asp.net core DI
        /// </summary>
        /// <param name="entity">IRequest or INotification</param>
        public static void StartNew(object entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            StartNew(static (sp, o) => OnMediator(sp.GetService<IMediator>(), o), entity);
        }

        private static Task OnMediator(IMediator mediator, object entity)
        {
            return entity switch
            {
                IBaseRequest _ => mediator.Send(entity),
                INotification _ => mediator.Publish(entity),
                _ => throw new InvalidOperationException("entity can't be handled by MediatorR"),
            };
        }
    }
}
