#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace ngen.Core.Security
{
    public enum PasswordStrength
    {
        TooPopular,
        VeryWeak,
        Weak,
        Ok,
        Strong,
        VeryStrong
    }

    public static class PasswordAdvisor
    {
        private static readonly string[] PopularPasswords =
        {
            "12345678", "qwerty", "abc123", "123456789", "111111", "123456", "iloveyou",
            "adobe123", "123123", "Admin", "1234567890", "letmein", "photoshop", "1234",
            "monkey", "shadow", "sunshine", "12345", "password1", "princess", "azerty",
            "trustno1", "000000", "pa55word", "Pa55word", "admin", "password"
        };

        public static PasswordStrength CalculateStrength(string password)
        {
            if (PopularPasswords.Any(p => p == password))
            {
                return PasswordStrength.TooPopular;
            }

            var entropy = CalculateShannonEntropy(password)*password.Length;


            if (entropy <= 19)
            {
                return PasswordStrength.VeryWeak;
            }

            if (entropy > 19 && entropy <= 39)
            {
                return PasswordStrength.Weak;
            }

            if (entropy > 39 && entropy <= 69)
            {
                return PasswordStrength.Ok;
            }

            if (entropy > 69 && entropy <= 119)
            {
                return PasswordStrength.Strong;
            }

            if (entropy > 119)
            {
                return PasswordStrength.VeryStrong;
            }

            return PasswordStrength.VeryWeak;
        }

        /// <summary>
        ///     returns bits of entropy represented in a given string, per
        ///     http://en.wikipedia.org/wiki/Entropy_(information_theory)
        /// </summary>
        private static double CalculateShannonEntropy(string s)
        {
            var map = new Dictionary<char, int>();

            foreach (var c in s)
            {
                if (!map.ContainsKey(c))
                    map.Add(c, 1);
                else
                    map[c] += 1;
            }

            var result = 0.0;
            var len = s.Length;

            foreach (var item in map)
            {
                var frequency = (double) item.Value/len;
                result -= frequency*(Math.Log(frequency)/Math.Log(2));
            }

            return result;
        }
    }
}