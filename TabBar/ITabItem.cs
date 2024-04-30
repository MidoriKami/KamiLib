namespace KamiLib.TabBar;

public interface ITabItem {
    string Name { get; set; }
    bool Enabled { get; set; }
    void Draw();
}