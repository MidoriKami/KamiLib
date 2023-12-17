using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Enums;
using KamiLib.KamiToolKit.Nodes;

namespace KamiLib.KamiToolKit.Interfaces;

public unsafe interface IResNode : IDisposable {
    AtkResNode* ResNode { get; }
    NodeType NodeType { get; }

    ITextNode AsTextNode() => (TextNode) this;
    IImageNode AsImageNode() => (ImageNode) this;
    
    uint NodeId { get; set; }

    void Enable();
    void Disable();
    void Toggle(bool enabled);
    
    Vector2 Size { get; set; }
    float Width { get; set; }
    float Height { get; set; }
    
    Vector2 Position { get; set; }
    float X { get; set; }
    float Y { get; set; }
    
    bool Visible { get; set; }
    
    Vector4 Color { get; set; }
    Vector3 AddColor { get; set; }
    Vector3 MultiplyColor { get; set; }
    
    float ScaleX { get; set; }
    float ScaleY { get; set; }
    Vector2 Scale { get; set; }
    float Rotation { get; set; }
}