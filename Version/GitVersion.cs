using System.Reflection;

namespace KamiLib.Version;

public static class GitVersion
{
    public static string VersionString()
    {
        var assemblyInformation = Assembly.GetExecutingAssembly().FullName!.Split(',');
        return assemblyInformation[1].Replace('=', ' ');
    }
}