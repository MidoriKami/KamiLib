using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.KamiToolKit.Interfaces;

public unsafe interface IResNode {
    AtkResNode* ResNode { get; }
    NodeType NodeType { get; }
    
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
}