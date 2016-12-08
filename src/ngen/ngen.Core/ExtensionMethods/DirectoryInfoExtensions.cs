#region Using directives

using System;
using System.IO;

#endregion

// ReSharper disable once CheckNamespace
public static class DirectoryInfoExtensions
{
    public static void DeleteEmptyDirs(this DirectoryInfo dir)
    {
        foreach (var d in dir.GetDirectories())
            d.DeleteEmptyDirs();

        try
        {
            dir.Delete();
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}