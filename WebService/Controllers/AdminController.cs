
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
        public ActionResult CheckAdmin()
        {
            // Like UserController.CheckLoggedIn, if the request gets this far, it's good.
            return new OkResult();
        }

        [HttpPost]
        [Route("frequency")]
        public async Task<ActionResult<Frequency>> AddFrequency([FromBody] FrequencyRequest request)
        {

            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            return new OkObjectResult(await _service.AddFrequencyAsync(request, userId));
        }

        [HttpPost]
        [Route("salarytype")]
        public async Task<ActionResult<SalaryType>> AddSalaryType([FromBody] SalaryTypeRequest request)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            return new OkObjectResult(await _service.AddSalaryTypeAsync(request, userId));
        }

        [HttpPost]
        [Route("transactiontype")]
        public async Task<ActionResult<TransactionType>> AddTransactionType([FromBody] TransactionTypeRequest request)
        {

            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            return new OkObjectResult(await _service.AddTransactionTypeAsync(request, userId));
        }

        [HttpPost]
        [Route("role")]
        public async Task<ActionResult<UserRole>> CreateUserRole([FromBody] UserRoleRequest request)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            return new OkObjectResult(await _service.AddUserRoleAsync(request, userId));
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