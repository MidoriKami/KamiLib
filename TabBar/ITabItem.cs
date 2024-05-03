namespace KamiLib.TabBar;

public interface ITabItem {
    string Name { get; }
    bool Enabled { get; set; }
    void Draw();
}