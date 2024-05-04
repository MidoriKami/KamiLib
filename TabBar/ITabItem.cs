namespace KamiLib.TabBar;

public interface ITabItem {
    string Name { get; }
    bool Disabled { get; }
    void Draw();
}