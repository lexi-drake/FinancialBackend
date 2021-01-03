using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private static CookieOptions Options = new CookieOptions() { Secure = true, HttpOnly = true, SameSite = SameSiteMode.Lax, IsEssential = true };
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
            SetCookies(token);
            return new OkResult();
        }

        [HttpGet]
        [Route("login")]
        // This needs to be authorized so that it will return an error if the user
        // is _not_ logged in.

        [Authorize]
        public ActionResult CheckLoggedIn()
        {
            // If the request gets this far, it's in the clear.
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
            SetCookies(newToken);
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
            SetCookies(token);
            return new OkResult();
        }

        [HttpGet]
        [Route("logout")]
        [Authorize]
        public async Task<ActionResult> LogoutUser()
        {
            // TODO (alexa): Delete refresh token from db.
            // The refresh token expires an hour after it was created, thanks to 
            // a TTL index, but we should probably go ahead and delete it here
            // just to be safe.
            ClearCookies();
            return new OkResult();
        }

        private void SetCookies(Token token)
        {
            // Set the cookie with the token values
            HttpContext.Response.Cookies.Append("jwt", token.Jwt, Options);
            HttpContext.Response.Cookies.Append("refresh", token.Refresh, Options);
        }

        private void ClearCookies()
        {
            HttpContext.Response.Cookies.Delete("jwt");
            HttpContext.Response.Cookies.Delete("refresh");
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
