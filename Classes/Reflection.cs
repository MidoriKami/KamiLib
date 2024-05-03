using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KamiLib.Classes;

public static class Reflection {
    public static IEnumerable<T> ActivateOfType<T>() 
        => Assembly.GetCallingAssembly()
            .GetTypes()
            .Where(type => type.IsSubclassOf(typeof(T)))
            .Where(type => !type.IsAbstract)
            .Select(type => (T?) Activator.CreateInstance(type))
            .OfType<T>();

    public static IEnumerable<T> ActivateOfInterface<T>() 
        => Assembly.GetCallingAssembly()
            .GetTypes()
            .Where(type => type.GetInterfaces().Contains(typeof(T)))
            .Where(type => !type.IsAbstract)
            .Select(type => (T?) Activator.CreateInstance(type))
            .OfType<T>();
}