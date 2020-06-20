namespace Miki.Discord.Common
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public static class EventExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task InvokeAsync<T>(this Func<T, Task> func, T arg)
        {
            return func != null ? func(arg) : Task.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task InvokeAsync<T1, T2>(this Func<T1, T2, Task> func, T1 arg1, T2 arg2)
        {
            return func != null ? func(arg1, arg2) : Task.CompletedTask;
        }
    }
}