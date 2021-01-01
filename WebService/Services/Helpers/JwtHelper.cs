using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace WebService
{
    public class JwtHelper
    {
        private const int LOGIN_MINUTES = 10;
        private const int REFRESH_TOKEN_SIZE = 32;
        private const string SECURITY_ALGORITHM = SecurityAlgorithms.HmacSha256Signature;
        private readonly string _secret;

        public JwtHelper(string secret)
        {
            _secret = secret;
        }

        public Token CreateToken(string userId, string roleId)
        {
            return new Token()
            {
                Jwt = CreateJwt(userId, roleId),
                Refresh = CreateRefresh()
            };
        }

        private string CreateJwt(string userid, string roleId)
        {
            if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(roleId))
            {
                return null;
            }

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, userid),
                new Claim(ClaimTypes.Role, roleId)
            };

            var expiration = DateTime.Now.AddMinutes(LOGIN_MINUTES);
            var credentials = new SigningCredentials(GetSecurityKey(), SECURITY_ALGORITHM);
            var jwt = new JwtSecurityToken("lexi", "drake", claims, expires: expiration, signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private SymmetricSecurityKey GetSecurityKey()
        {
            return new SymmetricSecurityKey(Convert.FromBase64String(_secret));
        }


        private string CreateRefresh(int size = REFRESH_TOKEN_SIZE)
        {
            var bytes = new byte[size];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public string GetUserIdFromToken(string jwt)
        {
            return GetClaimByType(jwt, ClaimTypes.Name)?.Value;
        }

        public string GetRoleFromToken(string jwt)
        {
            return GetClaimByType(jwt, ClaimTypes.Role)?.Value;
        }

        private Claim GetClaimByType(string jwt, string type)
        {
            var principal = GetPrincipalFromExpiredToken(jwt);
            var claims = from claim in principal.Claims
                         where claim.Type == type
                         select claim;
            if (claims.Any())
            {
                return null;
            }
            return claims.First();
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string jwt)
        {
            var parameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                IssuerSigningKey = GetSecurityKey(),
                ClockSkew = TimeSpan.Zero
            };
            return new JwtSecurityTokenHandler().ValidateToken(jwt, parameters, out var securityToken);
        }
    }
}