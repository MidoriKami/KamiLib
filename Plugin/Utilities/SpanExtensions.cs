using System;
using System.Runtime.CompilerServices;

namespace KamiLib.Utility;

public static class SpanExtensions
{
    public static unsafe T* Get<T>(this Span<T> span, int index) where T : unmanaged
    {
        return (T*) Unsafe.AsPointer(ref span[index]);
    }
}