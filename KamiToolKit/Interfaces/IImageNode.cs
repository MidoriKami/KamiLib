using System;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.KamiToolKit.Interfaces;

public interface IImageNode : IResNode {
    byte WrapMode { get; set; }
    
    ImageNodeFlags Flags { get; set; }
    
    Action? OnClick { set; }

    void LoadTexture(string texturePath);

    void LoadTexture(uint iconId);

    void UnloadTexture();
}