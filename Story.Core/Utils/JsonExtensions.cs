using System;

namespace Story.Core.Utils
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    // TODO: change back to internal
    public static class JsonExtensions
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        public static string Serialize(this object obj, bool indented = false)
        {
            if (obj == null)
            {
                return null;
            }

            if (indented)
            {
                return JsonConvert.SerializeObject(obj, Formatting.Indented);
            }

            return JsonConvert.SerializeObject(obj, JsonSerializerSettings);
        }
    }
}
