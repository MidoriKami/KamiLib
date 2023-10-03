using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Dalamud;
using Lumina.Excel;

namespace KamiLib.Game;

public class LuminaCache<T> : IEnumerable<T> where T : ExcelRow
{
    private static LuminaCache<T>? _instance;
    public static LuminaCache<T> Instance => _instance ??= new LuminaCache<T>();

    private readonly ConcurrentDictionary<uint, T> cache = new();
    private readonly Func<uint, T?> searchAction;
    private readonly ConcurrentDictionary<Tuple<uint, uint>, T> subRowCache = new();

    private LuminaCache(Func<uint, T?>? action = null)
    {
        searchAction = action ?? (row => Service.DataManager.GetExcelSheet<T>()!.GetRow(row));
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Service.DataManager.GetExcelSheet<T>()!.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public ExcelSheet<T> OfLanguage(ClientLanguage language)
    {
        return Service.DataManager.GetExcelSheet<T>(language)!;
    }

    public T? GetRow(uint id)
    {
        if (cache.TryGetValue(id, out var value))
        {
            return value;
        }
        if (searchAction(id) is not { } result) return null;

        return cache[id] = result;
    }

    public T? GetRow(uint row, uint subRow)
    {
        var targetRow = new Tuple<uint, uint>(row, subRow);

        if (subRowCache.TryGetValue(targetRow, out var value))
        {
            return value;
        }
        if (Service.DataManager.GetExcelSheet<T>()!.GetRow(row, subRow) is not { } result) return null;

        return subRowCache[targetRow] = result;
    }
}