#region Using directives

using System;
using System.Text;

#endregion

// ReSharper disable once CheckNamespace
public static class ByteArrayExtensions
{
    public static string ToHexString(this byte[] hex)
    {
        if (hex == null)
        {
            return null;
        }
        if (hex.Length == 0)
        {
            return string.Empty;
        }
        var s = new StringBuilder();
        foreach (var b in hex)
        {
            s.Append(b.ToString("x2"));
        }
        return s.ToString();
    }

    public static byte[] FromHexString(this string hexString)
    {
        if (hexString == null)
        {
            return null;
        }
        if (hexString.Length == 0)
        {
            return new byte[0];
        }
        var l = hexString.Length/2;
        var b = new byte[l];
        for (var i = 0; i < l; ++i)
        {
            b[i] = Convert.ToByte(hexString.Substring(i*2, 2), 16);
        }
        return b;
    }
}