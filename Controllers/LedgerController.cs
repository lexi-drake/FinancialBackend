using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace financial_backend.Controllers
{
    [ApiController]
    [Route("api/ledger")]
    public class LedgerController : ControllerBase
    {
        private readonly ILogger<LedgerController> _logger;

        public LedgerController(ILogger<LedgerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<LedgerEntry>> Get()
        {
            return new OkObjectResult(null);
        }
    }
}
