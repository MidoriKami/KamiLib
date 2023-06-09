namespace KamiLib.AutomaticUserInterface;

public class EnumLabel : AttributeBase
{
    private readonly string labelLocalizationKey;
    public string Label => TryGetLocalizedString(labelLocalizationKey);
    
    public EnumLabel(string label)
    {
        labelLocalizationKey = label;
    }
}