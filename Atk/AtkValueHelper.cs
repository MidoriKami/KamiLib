using System;
using System.Linq;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace KamiLib.Atk;

public class AtkValueHelper
{
    public static unsafe void PrintAtkArray(AtkValue* values, int count)
    {
        foreach (var index in Enumerable.Range(0, count))
        {
            PrintAtkValue(values[index], index);
        }
    }
    
    private static unsafe void PrintAtkValue(AtkValue value, int index)
    {
        switch (value.Type)
        {
            case ValueType.Int:
                Service.Log.Debug($"[{index:D3}] [{"int", 7}]: {value.Int}");
                break;
            case ValueType.Bool:
                Service.Log.Debug($"[{index:D3}] [{"bool", 7}]: {(value.Byte != 0 ? "true" : "false")}");
                break;
            case ValueType.UInt:
                Service.Log.Debug($"[{index:D3}] [{"uint", 7}]: {value.UInt}");
                break;
            case ValueType.Float:
                Service.Log.Debug($"[{index:D3}] [{"float", 7}]: {value.Float}");
                break;
            case ValueType.String:
                Service.Log.Debug($"[{index:D3}] [{"string", 7}]: {Marshal.PtrToStringUTF8(new nint(value.String))}");
                break;
            case ValueType.String8:
                Service.Log.Debug($"[{index:D3}] [{"string8", 7}]: {Marshal.PtrToStringUTF8(new nint(value.String))}");
                break;
            case ValueType.Vector:
                Service.Log.Debug($"[{index:D3}] [{"vector", 7}]: No Representation Implemented");
                break;
            case ValueType.AllocatedString:
                Service.Log.Debug($"[{index:D3}] [{"aString", 7}]: {Marshal.PtrToStringUTF8(new nint(value.String))}");
                break;
            case ValueType.AllocatedVector:
                Service.Log.Debug($"[{index:D3}] [{"aVector", 7}]: No Representation Implemented");
                break;
            default:                        
                Service.Log.Debug($"[{index:D3}] [{"unknown", 7}]: [{value.Type}]: {BitConverter.ToString(BitConverter.GetBytes((long)value.String)).Replace("-", " ")}");
                break;
        }
    }
}

public static class AtkValueExtensions
{
    public static unsafe string GetString(this AtkValue value)
    {
        return Marshal.PtrToStringUTF8(new nint(value.String)) ?? "Unable to Allocate String";
    }
}