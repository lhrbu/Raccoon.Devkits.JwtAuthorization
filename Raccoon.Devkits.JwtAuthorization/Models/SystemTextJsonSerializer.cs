using JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raccoon.Devkits.JwtAuthroization.Models
{
    class SystemTextJsonSerializer:IJsonSerializer
    {
        public string Serialize(object obj)=>JsonSerializer.Serialize(obj);
        

        public T Deserialize<T>(string json)
        {
            T? data = System.Text.Json.JsonSerializer.Deserialize<T>(json);

            if (data is Dictionary<string, object> odata)
            {
                Dictionary<string, object> ndata = new ();
                foreach (var key in odata.Keys)
                {
                    var value = (System.Text.Json.JsonElement)odata[key];
                    switch (value.ValueKind)
                    {
                        case JsonValueKind.Undefined:
                            break;
                        case JsonValueKind.Object:
                            break;
                        case JsonValueKind.Array:
                            break;
                        case JsonValueKind.String:
                            ndata.Add(key, value.GetString()!);
                            break;
                        case JsonValueKind.Number:
                            ndata.Add(key, value.GetInt64());
                            break;
                        case JsonValueKind.True:
                            break;
                        case JsonValueKind.False:
                            break;
                        case JsonValueKind.Null:
                            break;
                    }
                }

                if (ndata is T obj) return obj;
            }

            return data!;
        }
    }
}
