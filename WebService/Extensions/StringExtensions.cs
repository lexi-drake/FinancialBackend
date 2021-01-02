using System.Linq;

namespace WebService
{
    public static class StringExtensions
    {
        public static string ExtractJwtFromCookie(this string cookie)
        {
            if (cookie is null)
            {
                return "";
            }
            return cookie.Split(';').First().Trim();
        }
    }
}