using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedKernel.Extensions
{
    public static class ObjectExtensions
    {
        public static string OutputJson(this object item)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                //WriteIndented = true
            };
            var json = JsonSerializer.Serialize(item, serializeOptions);
            return json;
        }

        public static T OutputObject<T>(this string json)
        {
            var serializeOptions = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            };
            var result = JsonSerializer.Deserialize<T>(json, serializeOptions);
            return result;
        }
    }
}
