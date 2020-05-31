using System;
using System.Collections.Generic;
using System.Text;

namespace Resxul.Framework
{
    public static class StringExtensions
    {
        /// <summary>
        /// Determine if specified strings are null or empty or the strings are equal.
        /// </summary>
        /// <param name="a">String to compare</param>
        /// <param name="b">String to compare</param>
        /// <param name="comparisonType">StringComparison</param>
        /// <returns>True if the strings are null or empty or the strings are equal</returns>
        public static bool IsNullOrEmptyOrEquals(this string a, string b, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(a))
                return string.IsNullOrEmpty(b);

            return string.Equals(a, b, comparisonType);
        }
	}
}
