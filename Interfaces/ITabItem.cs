namespace KamiLib.Interfaces;

public interface ITabItem
{
    string TabName { get; }
    bool Enabled { get; }
    void Draw();
}