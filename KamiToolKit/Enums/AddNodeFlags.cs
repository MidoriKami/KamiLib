using System;

namespace KamiLib.KamiToolKit.Enums;

[Flags]
public enum AddNodeFlags {
    None = 0,
    IncrementNodeId = 1 << 0,
    OffsetPosition = 1 << 1,
    AdjustHeight = 1 << 2,
    AdjustWidth = 1 << 3,
    AdjustSize = 1 << 4,
    FillParent = 1 << 5,
}