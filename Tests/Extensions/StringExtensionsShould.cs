using System;
using Xunit;
using WebService;

namespace Tests
{
    public class StringExtensionsShould
    {
        [Fact]
        public void ReturnEmptyStringIfCookieIsNull()
        {
            string cookie = null;
            var value = cookie.ExtractJwtFromCookie();
            Assert.Equal("", value);
        }

        [Fact]
        public void ReturnsFirstSectionIfCookieIsNotNull()
        {
            var value = Guid.NewGuid().ToString();
            var cookie = $"key={value};httponly=true;secure=true";
            var cookieValue = cookie.ExtractJwtFromCookie();
            Assert.Equal($"key={value}", cookieValue);
        }
    }
}
