using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace WebService
{
    public class JwtHelper : IJwtHelper
    {
        private const int LOGIN_MINUTES = 10;
        private const int REFRESH_TOKEN_SIZE = 32;
        private const string SECURITY_ALGORITHM = SecurityAlgorithms.HmacSha256Signature;
        private readonly string _secret;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtHelper(string secret, string issuer, string audience)
        {
            _secret = secret;
            _issuer = issuer;
            _audience = audience;
        }

        public Token CreateToken(string userId, string role)
        {
            return new Token()
            {
                Jwt = CreateJwt(userId, role),
                Refresh = CreateRefresh()
            };
        }

        private string CreateJwt(string userid, string role)
        {
            if (string.IsNullOrEmpty(userid) || string.IsNullOrEmpty(role))
            {
                return null;
            }

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userid),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.Now.AddMinutes(LOGIN_MINUTES),
                NotBefore = DateTime.Now,
                IssuedAt = DateTime.Now,
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(GetSecurityKey(), SECURITY_ALGORITHM)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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
            try
            {
                return GetClaimByType(jwt, ClaimTypes.Name)?.Value;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public string GetRoleFromToken(string jwt)
        {
            try
            {
                return GetClaimByType(jwt, ClaimTypes.Role)?.Value;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }


        private Claim GetClaimByType(string jwt, string type)
        {
            var principal = GetPrincipalFromExpiredToken(jwt);
            var ids = principal.Claims;
            var claims = from id in ids
                         where id.Type == type
                         select id;

            if (!claims.Any())
            {
                return null;
            }
            return claims.First();
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string jwt)
        {
            var parameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_secret)),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };
            return new JwtSecurityTokenHandler().ValidateToken(jwt, parameters, out var securityToken);
        }
    }
}