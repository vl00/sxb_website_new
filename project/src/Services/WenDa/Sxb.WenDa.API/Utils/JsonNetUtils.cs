using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Sxb.WenDa.API.Utils
{
    internal static partial class JsonNetUtils
    {        
        public static Func<bool, JsonSerializerSettings> DefaultJsonSettingsFunc = (ignoreCase) =>
        {
            var options = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Error,
                TypeNameHandling = TypeNameHandling.None,
                ConstructorHandling = ConstructorHandling.Default
            };
            if (ignoreCase) options.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return options;
        };

        public static JsonSerializerSettings GetJsonSettings(bool ignoreCase = false)
        {
            var options = DefaultJsonSettingsFunc?.Invoke(ignoreCase);
            if (options == null)
            {
                options = new JsonSerializerSettings();
                if (ignoreCase) options.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
            return options;
        }

        public static JsonSerializerSettings AddConverter(this JsonSerializerSettings src, JsonConverter converter)
        {
            if (converter != null)
            {
                src.Converters ??= new List<JsonConverter>();
                src.Converters.Add(converter);
            }
            return src;
        }

        public static JsonSerializerSettings AddConverters(this JsonSerializerSettings src, IEnumerable<JsonConverter> converters)
        {
            if (converters != null)
            {
                src.Converters ??= new List<JsonConverter>();
                foreach (var converter in converters)
                {
                    if (converter != null)
                        src.Converters.Add(converter);
                }
            }
            return src;
        }

        public static string ToJsonStr(this object obj, bool ignoreCase = false, bool ignoreNull = false, bool indented = false,
            IEnumerable<JsonConverter> converters = null)
        {
            var options = GetJsonSettings(ignoreCase);
            options.NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include;
            if (indented) options.Formatting = Formatting.Indented;
            AddConverters(options, converters);

            return ToJsonStr(obj, options);
        }

        public static string ToJsonStr(this object obj, JsonSerializerSettings jsonSerializerSettings)
        {
            if (obj == null) return null; // null is default json to "null"
            return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
        }

        public static T JsonStrTo<T>(this string json, IEnumerable<JsonConverter> converters = null)
        {
            return (T)JsonStrTo(json, typeof(T), converters);
        }

        public static object JsonStrTo(this string json, Type type = null, IEnumerable<JsonConverter> converters = null)
        {
            var options = GetJsonSettings();
            AddConverters(options, converters);
            return JsonStrTo(json, type, options);
        }

        public static T JsonStrTo<T>(this string json, JsonSerializerSettings jsonSerializerSettings)
        {
            return (T)JsonStrTo(json, typeof(T), jsonSerializerSettings);
        }

        public static object JsonStrTo(this string json, Type type, JsonSerializerSettings jsonSerializerSettings)
        {
            try
            {
                return JsonConvert.DeserializeObject(json, type, jsonSerializerSettings);
            }
            catch (Exception ex)
            {                
                if (string.IsNullOrEmpty(json)) return null;
                throw new SerializationException(ex.Message, ex);
            }
        }
    }

}
