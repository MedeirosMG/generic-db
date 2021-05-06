using MyDB.Infrastructure.Tools.Interfaces;
using System;
using System.Text.Json;

namespace MyDB.Infrastructure.Tools
{
    public class UtilityService : IUtilityService
    {
        public UtilityService() { }
        public bool checkPrimitive(Type type) => (type.IsPrimitive || type.IsValueType || (type == typeof(string)));

        public T fromJson<T>(string value)
        {
            if (String.IsNullOrEmpty(value))
                return default(T);

            return JsonSerializer.Deserialize<T>(value);
        }

        public string toJson<T>(T value)
        {
            return JsonSerializer.Serialize(value);
        }        
    }
}
