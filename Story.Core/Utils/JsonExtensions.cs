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
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static readonly JsonSerializerSettings JsonSerializerIgnoreErrorSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Error = (sender, args) => { args.ErrorContext.Handled = true; }
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

        public static string SerializeIgnoreError(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            return JsonConvert.SerializeObject(obj, JsonSerializerIgnoreErrorSettings);
        }
    }
}
