using MiniECommerce.Services;
using System.Text.Json;

namespace MiniECommerce.Extensions
{
   
        public static class SessionExtensions
        {
            public static void SetCartCookie(this ISession session, string key, Cart value)
            {
                session.SetString(key, JsonSerializer.Serialize(value)); // covert object of type cart to jsonString
            }

            public static Cart GetCartCookie(this ISession session, string key)
            {
                var value = session.GetString(key);

                return value == null ? default : JsonSerializer.Deserialize<Cart>(value);
            }
        }
    }

