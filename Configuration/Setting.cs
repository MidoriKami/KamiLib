using System;
using System.Collections.Generic;

namespace KamiLib.Configuration;

public record Setting<T>(T Value) : IEquatable<T> where T : notnull 
{
    public T Value = Value;

    public override string ToString() => Value.ToString() ?? "Null";
    
    public virtual bool Equals(T? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return EqualityComparer<T>.Default.Equals(Value, other);
    }

    public static bool operator ==(Setting<T> leftSide, T rightSide)
    {
        return leftSide.Equals(rightSide);
    }
    
    public static bool operator !=(Setting<T> leftSide, T rightSide)
    {
        return !leftSide.Equals(rightSide);
    }
    
    public static implicit operator bool(Setting<T> obj)
    {
        if (obj.Value is bool value)
        {
            return value;
        }
        
        throw new Exception("Invalid implicit conversion to bool");
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return EqualityComparer<T>.Default.GetHashCode(Value);
    }
}