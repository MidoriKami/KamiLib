using System;
using System.Runtime.CompilerServices;

namespace KamiLib.Utilities;

public static class SpanExtensions
{
    public static unsafe T* Get<T>(this Span<T> span, int index) where T : unmanaged
        => (T*) Unsafe.AsPointer(ref span[index]);
}