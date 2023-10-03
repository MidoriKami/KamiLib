using System.ComponentModel;
using System.Drawing;

namespace KamiLib.AutomaticUserInterface;

public class DisplayColorAttribute : DescriptionAttribute
{
    public KnownColor Color { get; }

    public DisplayColorAttribute(KnownColor color)
    {
        Color = color;
    }
}