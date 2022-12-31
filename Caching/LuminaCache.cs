using System;
using System.Collections;
using System.Collections.Generic;
using Dalamud;
using Lumina.Excel;

namespace KamiLib.Caching;

public class LuminaCache<T> : IEnumerable<T> where T : ExcelRow
{
    private readonly Func<uint, T?> searchAction;

    private static LuminaCache<T>? _instance;
    public static LuminaCache<T> Instance => _instance ??= new LuminaCache<T>();

    protected LuminaCache(Func<uint, T?>? action = null)
    {
        searchAction = action ?? (row => Service.DataManager.GetExcelSheet<T>()!.GetRow(row));
    }

    private readonly Dictionary<uint, T> cache = new();
    
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
        else
        {
            if (searchAction(id) is not { } result) return null;
            
            return cache[id] = result;
        }
    }
    
    public IEnumerator<T> GetEnumerator() => Service.DataManager.GetExcelSheet<T>()!.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}