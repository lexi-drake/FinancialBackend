using System.Linq;

namespace WebService
{
    public static class StringExtensions
    {
        public static string ExtractJwtFromCookie(this string cookie) =>
            string.IsNullOrEmpty(cookie) ? "" : cookie.Split(';').First().Trim();
    }
}