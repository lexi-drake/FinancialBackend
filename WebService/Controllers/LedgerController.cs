using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/ledger")]
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

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<LedgerEntry>> InsertLedgerEntry([FromBody] LedgerEntryRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("categories")]
        public async Task<ActionResult<IEnumerable<LedgerEntryCategory>>> GetLedgerEntryCategories([FromBody] CategoryCompleteRequest request)
        {
            throw new NotImplementedException();
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
