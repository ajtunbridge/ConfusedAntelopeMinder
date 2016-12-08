#region Using directives

using System;

#endregion

// ReSharper disable once CheckNamespace
public static class EnumExtensions
{
    public static bool Has<T>(this Enum type, T value)
    {
        try
        {
            return ((int) (object) type & (int) (object) value) == (int) (object) value;
        }
        catch
        {
            return false;
        }
    }

    public static bool Is<T>(this Enum type, T value)
    {
        try
        {
            return (int) (object) type == (int) (object) value;
        }
        catch
        {
            return false;
        }
    }

    public static T Add<T>(this Enum type, T value)
    {
        try
        {
            return (T) (object) ((int) (object) type | (int) (object) value);
        }
        catch (Exception ex)
        {
            var message = string.Format("Could not append value from enumerated type '{0}'.", typeof (T).Name);

            throw new ArgumentException(message, ex);
        }
    }

    public static T Remove<T>(this Enum type, T value)
    {
        try
        {
            return (T) (object) ((int) (object) type & ~(int) (object) value);
        }
        catch (Exception ex)
        {
            var message = string.Format("Could not remove value from enumerated type '{0}'.", typeof (T).Name);

            throw new ArgumentException(message, ex);
        }
    }
}