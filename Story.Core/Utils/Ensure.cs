namespace Story.Core.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

    public static class Ensure
    {
		public static void ArgumentNotNull(object instance, string argumentName)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(argumentName);
			}
		}

		public static void ArgumentNotEmpty(string instance, string argumentName)
		{
			if (string.IsNullOrEmpty(instance))
			{
				throw new ArgumentException("'" + argumentName + "' cannot be null or empty!", argumentName);
			}
		}

		public static void ArgumentNotEmpty<T>(IEnumerable<T> collection, string argumentName)
		{
			if (collection == null || collection.Count() == 0)
			{
				throw new ArgumentException("'" + argumentName + "' cannot be null or empty!", argumentName);
			}
		}

        public static void ArgumentIs(bool condition, string argumentName)
        {
            if (!condition)
            {
                throw new ArgumentException("Condition failed for '" + argumentName + "'", argumentName);
            }
        }
    }
}
