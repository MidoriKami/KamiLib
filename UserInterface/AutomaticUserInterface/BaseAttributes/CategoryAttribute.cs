namespace KamiLib.AutomaticUserInterface;

public class CategoryAttribute : AttributeBase
{
    private readonly string categoryKey;
    public readonly int Index;
    
    public string Category => TryGetLocalizedString(categoryKey);

    public CategoryAttribute(string category, int index = 0)
    {
        categoryKey = category;
        Index = index;
    }
}