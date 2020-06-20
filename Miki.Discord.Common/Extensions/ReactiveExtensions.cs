namespace Miki.Discord.Common
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Threading.Tasks;

    public static class ReactiveExtensions
    {
        public static IDisposable SubscribeTask<T>(
            this IObservable<T> source,
            Func<T, Task> onNext)
        {
            return source
                .Select(e => Observable.Defer(() => onNext(e).ToObservable()))
                .Concat()
                .Subscribe(e => { });
        }
    }
}
