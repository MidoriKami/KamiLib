using System;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.KamiToolKit.Interfaces;

public interface ITextNode : IResNode {
    SeString Text { get; set; }
    
    byte FontSize { get; set; }
    
    Vector4 TextColor { get; set; }
    Vector4 OutlineColor { get; set; }
    Vector4 BackgroundColor { get; set; }
    
    AlignmentType TextAlignment { get; set; }
    TextFlags StyleFlags { get; set; }
    
    SeString? Tooltip { set; }
    Action? OnClick { set; }
}