namespace KamiLib.AutomaticUserInterface;

public class DrawCategory : AttributeBase
{
    public int DrawIndex { get; init; }

    public DrawCategory(string labelLocKey, int group) : base(labelLocKey)
    {
        DrawIndex = group;
    }
}