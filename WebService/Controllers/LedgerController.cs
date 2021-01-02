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

        public LedgerController(ILogger<LedgerController> logger, ILedgerService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<LedgerEntry>>> GetLegerEntries()
        {
            throw new NotImplementedException();
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
