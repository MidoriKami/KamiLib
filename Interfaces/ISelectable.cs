namespace KamiLib.Interfaces;

public interface ISelectable
{
    IDrawable Contents { get; }
    string ID { get; }
    void DrawLabel();
}