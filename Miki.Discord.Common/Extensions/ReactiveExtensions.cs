using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Miki.Discord.Common
{
    public static class ReactiveExtensions
    {
        /// <summary>
        /// Subscribes with an asynchronous task and handles errors in case of error.
        /// </summary>
        public static IDisposable SubscribeTask<T>(
            this IObservable<T> source,
            Func<T, Task> onNext,
            Func<Exception, Task> onError = default)
        {
            return source
                .Select(e => Observable.FromAsync(x => onNext(e))
                    .Catch<Unit, Exception>(err =>
                    {
                        if (onError != default)
                        {
                            onError(err);
                        }
                        return Observable.Return(Unit.Default);
                    }))
                .Concat()
                .Subscribe();
        }
    }
}
