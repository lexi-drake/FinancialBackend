using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger _logger;
        private IAdminService _service;
        private IJwtHelper _jwt;

        public AdminController(ILogger logger, IAdminService service, IJwtHelper jwt)
        {
            _logger = logger;
            _service = service;
            _jwt = jwt;
        }

        [HttpGet]
        [Route("")]
        // Like UserController.CheckLoggedIn, if the request gets this far, it's good.
        public ActionResult CheckAdmin() => new OkResult();

        [HttpPost]
        [Route("frequency")]
        public async Task<ActionResult> AddFrequency([FromBody] FrequencyRequest request)
        {

            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            await _service.AddFrequencyAsync(request, userId);
            return new OkResult();
        }

        [HttpPost]
        [Route("salarytype")]
        public async Task<ActionResult> AddSalaryType([FromBody] SalaryTypeRequest request)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            await _service.AddSalaryTypeAsync(request, userId);
            return new OkResult();
        }

        [HttpPost]
        [Route("transactiontype")]
        public async Task<ActionResult> AddTransactionType([FromBody] TransactionTypeRequest request)
        {

            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            await _service.AddTransactionTypeAsync(request, userId);
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
            await _service.AddUserRoleAsync(request, userId);
            return new OkResult();
        }

        [HttpPost]
        [Route("user/role")]
        public async Task<ActionResult> ChangeUserRole([FromBody] UpdateUserRoleRequest request)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            await _service.UpdateUserRoleAsync(request, userId);
            return new OkResult();
        }

        [HttpGet]
        [Route("tickets")]
        public async Task<ActionResult<IEnumerable<SupportTicketResponse>>> GetTickets() =>
            new OkObjectResult(await _service.GetSupportTicketsAsync());


        [HttpPost]
        [Route("ticket/{id}/resolve")]
        public async Task<ActionResult> ResolveTicket(string id)
        {
            await _service.ResolveSupportTicketAsync(id);
            return new OkResult();
        }

        [HttpPost]
        [Route("message")]
        public async Task<ActionResult> SubmitMessage([FromBody] MessageRequest request)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            await _service.AddMessageAsync(request, userId);
            return new OkResult();
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