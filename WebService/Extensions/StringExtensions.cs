using System.Linq;

namespace WebService
{
    public static class StringExtensions
    {
        public static string ExtractJwtFromCookie(this string cookie) =>
            cookie is null ? "" : cookie.Split(';').First().Trim();
    }
}