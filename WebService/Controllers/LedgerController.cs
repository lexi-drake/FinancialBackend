using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/ledger")]
    [Authorize]
    public class LedgerController : ControllerBase
    {
        private readonly ILogger<LedgerController> _logger;
        private ILedgerService _service;
        private JwtHelper _jwt;

        public LedgerController(ILogger<LedgerController> logger, ILedgerService service, JwtHelper jwt)
        {
            _logger = logger;
            _service = service;
            _jwt = jwt;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<LedgerEntry>>> GetLegerEntries()
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            return new OkObjectResult(await _service.GetLedgerEntriesByUserIdAsync(userId));
        }

        [HttpGet]
        [Route("{start}/{end}")]
        public async Task<ActionResult<IEnumerable<LedgerEntry>>> GetLegerEntries(string start, string end)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }

            var entries = new OkObjectResult(await _service.GetLedgerEntriesBetweenDatesAsync(start, end, userId));
            if (entries is null)
            {
                // null is only returned if the dates aren't parsable
                return new BadRequestResult();
            }
            return new OkObjectResult(entries);
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<LedgerEntry>> InsertLedgerEntry([FromBody] LedgerEntryRequest request)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            return new OkObjectResult(await _service.AddLedgerEntryAsync(request, userId));
        }

        [HttpGet]
        [Route("generators")]
        public async Task<ActionResult<IEnumerable<IncomeGeneratorResponse>>> GetIncomeGenerators()
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            return new OkObjectResult(await _service.GetIncomeGeneratorsByUserIdAsync(userId));
        }

        [HttpPost]
        [Route("generator")]
        public async Task<ActionResult<IncomeGeneratorResponse>> AddIncomeGenerator([FromBody] IncomeGeneratorRequest request)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            return new OkObjectResult(await _service.AddIncomeGeneratorAsync(request, userId));
        }

        [HttpPost]
        [Route("categories")]
        public async Task<ActionResult<IEnumerable<LedgerEntryCategory>>> GetLedgerEntryCategories([FromBody] CategoryCompleteRequest request)
        {
            return new OkObjectResult(await _service.GetLedgerEntryCategoriesLikeAsync(request));
        }

        [HttpGet]
        [Route("frequencies")]
        public async Task<ActionResult<IEnumerable<Frequency>>> GetFrequencies()
        {
            return new OkObjectResult(await _service.GetAllAsync<Frequency>());
        }

        [HttpGet]
        [Route("salarytypes")]
        public async Task<ActionResult<IEnumerable<SalaryType>>> GetSalaryTypes()
        {
            return new OkObjectResult(await _service.GetAllAsync<SalaryType>());
        }

        [HttpGet]
        [Route("transactiontypes")]
        public async Task<ActionResult<IEnumerable<TransactionType>>> GetTransactionTypes()
        {
            return new OkObjectResult(await _service.GetAllAsync<TransactionType>());
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
