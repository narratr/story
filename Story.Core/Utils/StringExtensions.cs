using System;

namespace Story.Core.Utils
{
    /// <summary>
    /// Extension methods for string operations.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Truncates a string to the specified maximum length and adds "..." if truncated.
        /// </summary>
        /// <param name="value">The string to truncate.</param>
        /// <param name="maxLength">The maximum length of the returned string (including "...").</param>
        /// <returns>The original string if it's within the length limit, or a truncated string with "..." appended.</returns>
        /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
        /// <exception cref="ArgumentException">Thrown when maxLength is less than 3 (minimum needed for "...").</exception>
        public static string TruncateWithEllipsis(this string value, int maxLength)
        {
            if (value == null)
            {
                return value;
            }

            if (value.Length <= maxLength)
            {
                return value;
            }

            return value.Substring(0, maxLength) + "...";
        }

        /// <summary>
        /// Truncates the string representation of an object to the specified maximum length and adds "..." if truncated.
        /// </summary>
        /// <param name="obj">The object whose string representation will be truncated.</param>
        /// <param name="maxLength">The maximum length of the returned string representation (including "...").</param>
        /// <returns>
        /// The original string representation if it's within the length limit, 
        /// or a truncated string with "..." appended. 
        /// Returns null if the object is null.
        /// </returns>
        public static object TruncateWithEllipsis(this object obj, int maxLength)
        {
            return obj?.ToString().TruncateWithEllipsis(maxLength);
        }
    }
}
