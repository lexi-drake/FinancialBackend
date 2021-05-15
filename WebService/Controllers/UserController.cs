using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Serilog;
using MediatR;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private static CookieOptions Options = new CookieOptions() { Secure = true, HttpOnly = true, SameSite = SameSiteMode.None, IsEssential = true };
        private readonly ILogger _logger;
        private IMediator _mediatr;

        public UserController(ILogger logger, IMediator mediatr)
        {
            _logger = logger;
            _mediatr = mediatr;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<long>> GetUserCount() =>
            new OkObjectResult(await _mediatr.Send(new GetUserCountQuery()));


        [HttpPut]
        [Route("login")]
        public async Task<ActionResult<LoginResponse>> LoginUser([FromBody] LoginRequest request)
        {
            try
            {
                var loginResponse = await _mediatr.Send(new LoginQuery()
                {
                    Request = request
                });
                SetCookies(loginResponse.Token);
                return new OkObjectResult(loginResponse);
            }
            catch (ArgumentException)
            {
                return new UnauthorizedResult();
            }
        }

        [HttpGet]
        [Route("login")]
        // This needs to be authorized so that it will return an error if the user
        // is _not_ logged in.
        [Authorize]
        public async Task<ActionResult<LoginResponse>> CheckLoggedIn()
        {
            try
            {
                var token = GetTokenFromCookie();
                var loginResponse = await _mediatr.Send(new CheckLoginStatusQuery()
                {
                    Token = token
                });
                return new OkObjectResult(loginResponse);
            }
            catch (ArgumentException)
            {
                return new UnauthorizedResult();
            }
        }

        [HttpGet]
        [Route("refresh")]
        // Not authorized because the jwt will likely be expired when this 
        // endpoint is targeted.
        public async Task<ActionResult> RefreshToken()
        {
            var token = GetTokenFromCookie();
            try
            {
                var newToken = await _mediatr.Send(new RefreshTokenQuery()
                {
                    Token = token
                });
                SetCookies(newToken);
                return new OkResult();
            }
            catch (ArgumentException)
            {
                return new UnauthorizedResult();
            }
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<LoginResponse>> CreateUser([FromBody] CreateUserRequest request)
        {
            await _mediatr.Send(new CreateUserCommand()
            {
                Request = request
            });

            try
            {
                var loginResponse = await _mediatr.Send(new LoginQuery()
                {
                    Request = new LoginRequest()
                    {
                        Username = request.Username,
                        Password = request.Password
                    }
                });
                SetCookies(loginResponse.Token);
                return new OkObjectResult(loginResponse);
            }
            catch (ArgumentException)
            {
                // Realistically, this should never be reached because it's
                // using the same data that was just used to create the user
                // to log that user in. If this hits, it's indicative of a more
                // serious problem (or connectivity issues with the db).
                return new UnauthorizedResult();
            }
        }

        [HttpDelete]
        [Route("logout")]
        [Authorize]
        public async Task<ActionResult> LogoutUser()
        {
            var token = GetTokenFromCookie();
            try
            {
                await _mediatr.Send(new LogoutCommand()
                {
                    Token = token
                });
                ClearCookies();
                return new OkResult();
            }
            catch (ArgumentException)
            {
                return new UnauthorizedResult();
            }
        }

        // [HttpPost]
        // [Route("edit/username")]
        // [Authorize]
        // public async Task<ActionResult<UpdateUsernameResponse>> UpdateUsername([FromBody] UpdateUsernameRequest request)
        // {
        //     var token = GetTokenFromCookie();
        //     var response = await _service.UpdateUsernameAsync(request, token);
        //     if (response is null)
        //     {
        //         return new NotFoundResult();
        //     }
        //     return new OkObjectResult(response);
        // }

        [HttpPost]
        [Route("ticket")]
        [Authorize]
        public async Task<ActionResult> SubmitSupportTicket([FromBody] SupportTicketRequest request)
        {
            var token = GetTokenFromCookie();
            try
            {
                await _mediatr.Send(new AddSupportTicketCommand()
                {
                    Request = request,
                    Token = token
                });
                return new OkResult();
            }
            catch (ArgumentException)
            {
                return new UnauthorizedResult();
            }
        }

        [HttpPatch]
        [Route("message")]
        public async Task<ActionResult> SubmitMessage([FromBody] MessageRequest request)
        {
            var token = GetTokenFromCookie();
            try
            {
                await _mediatr.Send(new AddMessageCommand()
                {
                    Token = token,
                    Request = request
                });
                return new OkResult();
            }
            catch (ArgumentException)
            {
                return new NotFoundResult();
            }
        }

        [HttpGet]
        [Route("tickets")]
        public async Task<ActionResult<IEnumerable<SupportTicketResponse>>> GetTickets()
        {
            var token = GetTokenFromCookie();
            try
            {
                var tickets = await _mediatr.Send(new GetTicketsQuery()
                {
                    Token = token
                });
                return tickets is null ? new NotFoundResult() : new OkObjectResult(tickets);
            }
            catch (ArgumentException)
            {
                return new NotFoundResult();
            }
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
