using System;
using System.Runtime.CompilerServices;

namespace KamiLib.Utilities;

public static class SpanExtensions
{
    public static unsafe T* Get<T>(this Span<T> span, int index) where T : unmanaged 
        => (T*) Unsafe.AsPointer(ref span[index]);
    
    public unsafe ref struct SpanEnumerator<T> where T : unmanaged {
        private int currentIndex;
        private readonly Span<T> items;
        public SpanEnumerator(Span<T> span) => items = span;
        public bool MoveNext() => ++currentIndex < items.Length;
        public readonly T* Current => (T*) Unsafe.AsPointer(ref items[currentIndex]);
        public SpanEnumerator<T> GetEnumerator() => new(items);
    }

    public static SpanEnumerator<T> Enumerator<T>(this Span<T> span) where T : unmanaged => new(span);
}