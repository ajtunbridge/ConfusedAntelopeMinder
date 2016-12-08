#region Using directives

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

#endregion

// ReSharper disable once CheckNamespace
public static class StringExtensions
{
    #region Constants

    /// <summary>
    ///     Source of pattern:
    ///     http://www.rhyous.com/2010/06/15/regular-expressions-in-cincluding-a-new-comprehensive-email-pattern/
    /// </summary>
    private const string EmailRegex =
        @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

    #endregion

    /// <summary>
    ///     Returns true if this string is null, empty or consists of only whitespace characters
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsNullOrWhitespace(this string s)
    {
        return string.IsNullOrWhiteSpace(s);
    }

    /// <summary>
    ///     Determines if this string is a number
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsNumeric(this string s)
    {
        double result = 0;

        return double.TryParse(s, out result);
    }

    /// <summary>
    ///     Converts this string to Title Case
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string ToTitleCase(this string s)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);
    }

    public static byte[] ToBytes(this string s)
    {
        return Encoding.UTF8.GetBytes(s);
    }

    public static bool IsValidEmailAddress(this string value)
    {
        var regex = new Regex(EmailRegex);
        return regex.IsMatch(value);
    }
}