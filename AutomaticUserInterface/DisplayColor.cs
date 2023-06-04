using System.ComponentModel;
using System.Drawing;

namespace KamiLib.AutomaticUserInterface;

public class DisplayColor : DescriptionAttribute
{
    public KnownColor Color { get; }

    public DisplayColor(KnownColor color)
    {
        Color = color;
    }
}
