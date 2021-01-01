using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet]
        [Route("categories")]
        public async Task<ActionResult<IEnumerable<LedgerEntryCategory>>> GetLedgerEntryCategories()
        {
            throw new NotImplementedException();
        }
    }
}
