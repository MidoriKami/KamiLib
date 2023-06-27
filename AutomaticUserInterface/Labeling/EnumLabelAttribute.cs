namespace KamiLib.AutomaticUserInterface;

public class EnumLabelAttribute : AttributeBase
{
    private readonly string labelLocalizationKey;
    public string Label => TryGetLocalizedString(labelLocalizationKey);
    
    public EnumLabelAttribute(string label)
    {
        labelLocalizationKey = label;
    }
}