using System;

namespace Story.Core.Utils
{
    /// <summary>
    /// Utility class for generating instance IDs with a consistent format.
    /// </summary>
    public static class InstanceIdGenerator
    {
        /// <summary>
        /// Generates an instance ID with the format: {name}_{YYMMDD}_{NeEwGuid}
        /// </summary>
        /// <param name="name">The name to include in the instance ID</param>
        /// <returns>A formatted instance ID string</returns>
        public static string GenerateInstanceId(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "Unknown";
            }

            // Get current date in YYMMDD format
            var dateString = DateTime.UtcNow.ToString("yyMMdd");
            
            // Generate a new GUID and convert to string
            var guid = Guid.NewGuid().ToString("N"); // "N" format removes hyphens
            
            return $"{name}_{dateString}_{guid}";
        }
    }
}
