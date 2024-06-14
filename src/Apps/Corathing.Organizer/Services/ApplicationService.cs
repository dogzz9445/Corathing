using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Organizer.Services
{
    public class ApplicationService : IApplicationService
    {
        public Task<TResult> InvokeAsync<TResult>(Func<TResult> callback)
        {
            var dispatcherOpeartion = App.Current.Dispatcher.InvokeAsync(callback);
            return dispatcherOpeartion.Task;
        }

        public async Task<TResult> DispatchAsync<TResult>(Func<Task<TResult>> callback)
        {
            var result = await App.Current.Dispatcher.InvokeAsync(callback);
            return result.Result;
        }
    }
}
