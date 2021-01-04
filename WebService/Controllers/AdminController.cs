
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private IAdminService _service;
        private JwtHelper _jwt;

        public AdminController(ILogger<AdminController> logger, IAdminService service, JwtHelper jwt)
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
            // TODO (alexa): Should this prevent the user from changing their own role?
            // Right now this function requires admin access, so it's not like a User could
            // change their own role to Admin, but it potentially allows for Admins to change
            // themselves into regular Users (which, sure... I guess that's ok for now? The
            // real risk is that the LAST Admin would make themselves a regular User...).
            // This is really a bigger issue if another role is added that provides different
            // functionality that Admins don't intrinsically have, which would be a little 
            // weird in itself.
            await _service.UpdateUserRoleAsync(request);
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