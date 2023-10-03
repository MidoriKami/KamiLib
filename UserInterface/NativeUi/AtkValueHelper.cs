using System;
using System.Runtime.InteropServices;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace KamiLib.NativeUi;

public class AtkValueHelper
{
    public static unsafe void PrintAtkArray(AtkValue* values, int count)
    {
        new Span<AtkValue>(values, count).PrintToLog();
    }
}

public static class AtkValueExtensions
{
    public static void PrintToLog(this Span<AtkValue> span)
    {
        foreach (var value in span) value.PrintToLog();
    }

    public static unsafe string GetString(this AtkValue value)
    {
        return MemoryHelper.ReadStringNullTerminated(new nint(value.String));
    }

    public static void PrintToLog(this AtkValue value)
    {
        Service.Log.Debug(value.GetValueDebugString());
    }

    private static unsafe string GetValueDebugString(this AtkValue value)
    {
        return value.Type switch
        {
            ValueType.Int => $"[{"int",7}]: {value.Int}",
            ValueType.Bool => $"[{"bool",7}]: {(value.Byte != 0 ? "true" : "false")}",
            ValueType.UInt => $"[{"uint",7}]: {value.UInt}",
            ValueType.Float => $"[{"float",7}]: {value.Float}",
            ValueType.String => $"[{"string",7}]: {MemoryHelper.ReadStringNullTerminated(new nint(value.String))}",
            ValueType.String8 => $"[{"string8",7}]: {MemoryHelper.ReadStringNullTerminated(new nint(value.String))}",
            ValueType.Vector => $"[{"vector",7}]: No Representation Implemented",
            ValueType.AllocatedString => $"[{"aString",7}]: {Marshal.PtrToStringUTF8(new nint(value.String))}",
            ValueType.AllocatedVector => $"[{"aVector",7}]: No Representation Implemented",
            _ => $"[{"unknown",7}]: [{value.Type}]: {BitConverter.ToString(BitConverter.GetBytes((long) value.String)).Replace("-", " ")}"
        };
    }
}