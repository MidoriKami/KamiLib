namespace KamiLib.Configuration;

public record Setting<T>(T Value)
{
    public T Value = Value;

    public override string ToString() => Value?.ToString() ?? "Null";
}
