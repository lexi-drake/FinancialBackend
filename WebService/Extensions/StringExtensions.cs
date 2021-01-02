using System.Linq;

namespace WebService
{
    public static class StringExtensions
    {
        public static string ExtractJwtFromCookie(this string cookie)
        {
            return cookie.Split(';').First().Trim();
        }
    }
}