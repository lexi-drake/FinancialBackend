using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using MediatR;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger _logger;
        private IMediator _mediatr;
        private IJwtHelper _jwt;

        public AdminController(ILogger logger, IMediator mediatr, IJwtHelper jwt)
        {
            _logger = logger;
            _mediatr = mediatr;
            _jwt = jwt;
        }

        [HttpGet]
        [Route("")]
        // Like UserController.CheckLoggedIn, if the request gets this far, it's good.
        public ActionResult CheckAdmin() => new OkResult();

        [HttpPost]
        [Route("frequency")]
        public async Task<ActionResult> AddFrequency([FromBody] FrequencyRequest request) =>
            await AddLedgerItem(new Frequency()
            {
                Description = request.Description,
                ApproxTimesPerYear = request.ApproxTimesPerYear
            });

        [HttpPost]
        [Route("salarytype")]
        public async Task<ActionResult> AddSalaryType([FromBody] SalaryTypeRequest request) =>
            await AddLedgerItem(new SalaryType()
            {
                Description = request.Description
            });

        [HttpPost]
        [Route("transactiontype")]
        public async Task<ActionResult> AddTransactionType([FromBody] TransactionTypeRequest request) =>
            await AddLedgerItem(new TransactionType()
            {
                Description = request.Description
            });

        private async Task<ActionResult> AddLedgerItem(AbstractLedgerItem item)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            await _mediatr.Send(new AddLedgerItemCommand()
            {
                UserId = userId,
                LedgerItem = item
            });
            return new OkResult();
        }

        [HttpPost]
        [Route("role")]
        public async Task<ActionResult> CreateUserRole([FromBody] UserRoleRequest request)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            await _mediatr.Send(new AddUserRoleCommand()
            {
                Description = request.Description,
                CreatedBy = userId
            });
            return new OkResult();
        }

        // [HttpPost]
        // [Route("user/role")]
        // public async Task<ActionResult> ChangeUserRole([FromBody] UpdateUserRoleRequest request)
        // {
        //     var userId = GetUserIdFromCookie();
        //     if (userId is null)
        //     {
        //         return new UnauthorizedResult();
        //     }
        //     await _service.UpdateUserRoleAsync(request, userId);
        //     return new OkResult();
        // }

        [HttpPost]
        [Route("ticket/{id}/resolve")]
        public async Task<ActionResult> ResolveTicket(string id)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }

            try
            {
                await _mediatr.Send(new ResolveSupportTicketCommand()
                {
                    Id = id,
                    UserId = userId
                });
                return new OkResult();
            }
            catch (ArgumentException)
            {
                return new NotFoundResult();
            }
        }

        private string GetUserIdFromCookie()
        {
            if (!HttpContext.Request.Cookies.TryGetValue("jwt", out var jwt))
            {
                return null;
            }
            return _jwt.GetUserIdFromToken(jwt);
        }
    }
}