using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private static CookieOptions Options = new CookieOptions() { Secure = true, HttpOnly = true, SameSite = SameSiteMode.Strict };
        private readonly ILogger<UserController> _logger;
        private IUserService _service;

        public UserController(ILogger<UserController> logger, IUserService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> LoginUser([FromBody] LoginRequest request)
        {
            var token = await _service.LoginUserAsync(request);
            if (token is null)
            {
                return new NotFoundResult();
            }
            PrepareCookies(token);
            return new OkResult();
        }

        [HttpGet]
        [Route("refresh")]
        public async Task<ActionResult> RefreshToken()
        {
            var token = GetTokenFromCookie();
            if (token is null)
            {
                return new UnauthorizedResult();
            }
            var newToken = await _service.RefreshLoginAsync(token);
            PrepareCookies(newToken);
            return new OkResult();
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var token = await _service.CreateUserAsync(request);
            if (token is null)
            {
                return new NotFoundResult();
            }
            PrepareCookies(token);
            return new OkResult();
        }

        private void PrepareCookies(Token token)
        {
            // Delete existing cookies, useful for refreshing
            HttpContext.Response.Cookies.Delete("jwt");
            HttpContext.Response.Cookies.Delete("refresh");

            // Set the cookie with the token values
            HttpContext.Response.Cookies.Append("jwt", token.Jwt, Options);
            HttpContext.Response.Cookies.Append("refresh", token.Refresh, Options);
        }

        private Token GetTokenFromCookie()
        {
            if (!HttpContext.Request.Cookies.TryGetValue("jwt", out var jwt) || !HttpContext.Request.Cookies.TryGetValue("refresh", out var refresh))
            {
                return null;
            }
            return new Token()
            {
                Jwt = jwt,
                Refresh = refresh
            };
        }
    }
}
