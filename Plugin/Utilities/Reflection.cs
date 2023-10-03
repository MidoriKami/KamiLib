using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KamiLib.Utility;

public static class Reflection
{
    public static IEnumerable<T> ActivateOfType<T>()
    {
        return Assembly.GetCallingAssembly()
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(T)))
            .Where(type => !type.IsAbstract)
            .Select(type => (T?) Activator.CreateInstance(type))
            .OfType<T>();
    }

    public static IEnumerable<T> ActivateOfInterface<T>()
    {
        return Assembly.GetCallingAssembly()
            .GetTypes()
            .Where(type => type.GetInterfaces().Contains(typeof(T)))
            .Where(type => !type.IsAbstract)
            .Select(type => (T?) Activator.CreateInstance(type))
            .OfType<T>();
    }
}