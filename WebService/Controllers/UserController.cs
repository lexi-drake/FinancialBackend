using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private static CookieOptions Options = new CookieOptions() { Secure = true, HttpOnly = true, SameSite = SameSiteMode.None, IsEssential = true };
        private readonly ILogger _logger;
        private IUserService _service;

        public UserController(ILogger logger, IUserService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<long>> GetUserCount()
        {
            return new OkObjectResult(await _service.GetUserCountAsync());
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginResponse>> LoginUser([FromBody] LoginRequest request)
        {
            var loginResponse = await _service.LoginUserAsync(request);
            if (loginResponse is null)
            {
                return new NotFoundResult();
            }
            SetCookies(loginResponse.Token);
            return new OkObjectResult(loginResponse);
        }

        [HttpGet]
        [Route("login")]
        // This needs to be authorized so that it will return an error if the user
        // is _not_ logged in.
        [Authorize]
        public async Task<ActionResult<LoginResponse>> CheckLoggedIn()
        {
            var token = GetTokenFromCookie();
            var loginResponse = await _service.GetUserAsync(token);
            if (loginResponse is null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(loginResponse);
        }

        [HttpGet]
        [Route("refresh")]
        // Not authorized because the jwt will likely be expired when this 
        // endpoint is targeted.
        public async Task<ActionResult> RefreshToken()
        {
            var token = GetTokenFromCookie();
            var newToken = await _service.RefreshLoginAsync(token);
            if (newToken is null)
            {
                return new UnauthorizedResult();
            }
            SetCookies(newToken);
            return new OkResult();
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<LoginResponse>> CreateUser([FromBody] CreateUserRequest request)
        {
            var loginResponse = await _service.CreateUserAsync(request);
            if (loginResponse is null)
            {
                return new NotFoundResult();
            }
            SetCookies(loginResponse.Token);
            return new OkObjectResult(loginResponse);
        }

        [HttpGet]
        [Route("logout")]
        [Authorize]
        public async Task<ActionResult> LogoutUser()
        {
            var token = GetTokenFromCookie();
            ActionResult result = new OkResult();
            try
            {
                // This throws an ArgumentException if the jwt is empty
                await _service.LogoutUserAsync(token);
            }
            catch (ArgumentException ex)
            {
                // Don't throw an exception here because we want to propogate the 
                // error message to the front end instead of just sending a 500.
                result = new UnauthorizedObjectResult(ex.Message);
            }
            ClearCookies();
            return result;
        }

        [HttpPost]
        [Route("edit/username")]
        [Authorize]
        public async Task<ActionResult<UpdateUsernameResponse>> UpdateUsername([FromBody] UpdateUsernameRequest request)
        {
            var token = GetTokenFromCookie();
            var response = await _service.UpdateUsernameAsync(request, token);
            if (response is null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(response);
        }

        [HttpPost]
        [Route("ticket")]
        [Authorize]
        public async Task<ActionResult> SubmitSupportTicket([FromBody] SupportTicketRequest request)
        {
            var token = GetTokenFromCookie();
            ActionResult result = new OkResult();
            try
            {
                await _service.SubmitSupportTicketAsync(request, token);
            }
            catch (ArgumentException ex)
            {
                result = new UnauthorizedObjectResult(ex.Message);
            }
            return result;
        }

        [HttpGet]
        [Route("messages")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MessageResponse>>> GetMessages()
        {
            var token = GetTokenFromCookie();
            var response = await _service.GetMessagesAsync(token);
            if (response is null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(response);
        }

        private void SetCookies(Token token)
        {
            Options.Expires = new DateTimeOffset(DateTime.Now.AddMinutes(15));
            HttpContext.Response.Cookies.Append("jwt", token.Jwt, Options);
            HttpContext.Response.Cookies.Append("refresh", token.Refresh, Options);
        }

        private void ClearCookies()
        {
            // This doesn't actually delete the cookie, it returns an empty value
            // to the client so that the client deletes the cookie.
            Options.Expires = new DateTimeOffset(DateTime.Now.AddMinutes(1));
            HttpContext.Response.Cookies.Delete("jwt", Options);
            HttpContext.Response.Cookies.Delete("refresh", Options);
        }

        private Token GetTokenFromCookie()
        {
            HttpContext.Request.Cookies.TryGetValue("jwt", out var jwt);
            HttpContext.Request.Cookies.TryGetValue("refresh", out var refresh);

            return new Token()
            {
                Jwt = jwt,
                Refresh = refresh
            };
        }
    }
}
