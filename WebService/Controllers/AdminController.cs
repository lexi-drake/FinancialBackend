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