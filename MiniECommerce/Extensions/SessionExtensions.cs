using MiniECommerce.Services;
using System.Text.Json;

namespace MiniECommerce.Extensions
{
   
        public static class SessionExtensions
        {
            public static void SetCartCookie(this ISession session, string key, Cart value) // Saving 
            {
                session.SetString(key, JsonSerializer.Serialize(value)); // covert object of type cart to jsonString
            }

            public static Cart? GetCartCookie(this ISession session, string key) // Reading
            {
                var value = session.GetString(key);
                if(value != null)
                {
                    var CartCookie = JsonSerializer.Deserialize<Cart>(value);
                    return CartCookie;
                }

                return null;
            }
        }
    }

