using System;
using System.Collections.Generic;
using Lumina.Excel;

namespace KamiLib.Caching;

public class LuminaCache<T> where T : ExcelRow
{
    private readonly Func<uint, T> searchAction;

    public LuminaCache(Func<uint, T>? action = null)
    {
        searchAction = action ?? (row => Service.DataManager.GetExcelSheet<T>()!.GetRow(row)!);
    }

    private readonly Dictionary<uint, T> cache = new();

    public T GetRow(uint id)
    {
        if (cache.TryGetValue(id, out var value))
        {
            return value;
        }
        else
        {
            return cache[id] = searchAction(id);
        }
    }
}