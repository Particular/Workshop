using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Divergent.Frontend.ITOps
{
    public class CamelCaseToPascalSettings
    {
        static JsonSerializerSettings settings;

        static CamelCaseToPascalSettings()
        {
            settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new CamelCaseToPascalCaseExpandoObjectConverter() }
            };
        }

        public static JsonSerializerSettings GetSerializerSettings()
        {
            return settings;
        }
    }
}
