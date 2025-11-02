using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ShoesShop.Crosscutting.Utilities.Session
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T? Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }

        public static void Remove(this ISession session, string key)
        {
            session.Remove(key);
        }
    }
}