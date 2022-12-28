using System;

namespace KamiLib.Interfaces;

public interface ITabItem : IDisposable
{
    string TabName { get; }
    bool Enabled { get; }
    void Draw();
}