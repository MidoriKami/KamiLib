namespace KamiLib.Interfaces;

public interface ISelectable
{
    IDrawable Contents { get; }
    void DrawLabel();
    string ID { get; }
}