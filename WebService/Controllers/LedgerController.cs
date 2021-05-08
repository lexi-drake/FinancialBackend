using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using MediatR;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/ledger")]
    [Authorize]
    public class LedgerController : ControllerBase
    {
        private readonly ILogger _logger;
        private IMediator _mediatr;
        private IJwtHelper _jwt;

        public LedgerController(ILogger logger, IMediator mediatr, IJwtHelper jwt)
        {
            _logger = logger;
            _mediatr = mediatr;
            _jwt = jwt;
        }

        [HttpGet]
        [Route("{start}/{end}")]
        public async Task<ActionResult<IEnumerable<LedgerEntryResponse>>> GetLegerEntries(string start, string end)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            try
            {
                var entries = await _mediatr.Send(new GetLedgerEntriesQuery()
                {
                    UserId = userId,
                    Start = start,
                    End = end
                });
                return new OkObjectResult(entries);
            }
            catch (ArgumentException)
            {
                return new NotFoundResult();
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult> InsertLedgerEntry([FromBody] LedgerEntryRequest request)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            await _mediatr.Send(new AddLedgerEntryCommand()
            {
                UserId = userId,
                Request = request
            });
            return new OkResult();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteLedgerEntry(string id)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            await _mediatr.Send(new DeleteLedgerEntryCommand()
            {
                UserId = userId,
                Id = id
            });
            return new OkResult();
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
            var generators = await _mediatr.Send(new GetIncomeGeneratorsQuery()
            {
                UserId = userId
            });
            return new OkObjectResult(generators);
        }

        [HttpPost]
        [Route("generator")]
        public async Task<ActionResult> AddIncomeGenerator([FromBody] IncomeGeneratorRequest request)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }

            var transactionIds = new List<string>();
            foreach (var transaction in request.RecurringTransactions)
            {
                transactionIds.Add(await _mediatr.Send(new AddRecurringTransactionQuery()
                {
                    UserId = userId,
                    Request = transaction
                }));
            }

            await _mediatr.Send(new AddIncomeGeneratorCommand()
            {
                UserId = userId,
                Request = request,
                TransactionIds = transactionIds
            });
            return new OkResult();
        }

        [HttpDelete]
        [Route("generator/{id}")]
        public async Task<ActionResult> DeleteIncomeGenerator(string id)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            try
            {
                var transactionIds = await _mediatr.Send(new DeleteIncomeGeneratorQuery()
                {
                    UserId = userId,
                    Id = id
                });
                foreach (var transactionId in transactionIds)
                {
                    await _mediatr.Send(new DeleteRecurringTransactionCommand()
                    {
                        UserId = userId,
                        Id = transactionId
                    });
                }

                return new OkResult();
            }
            catch (ArgumentException)
            {
                return new NotFoundResult();
            }
        }

        [HttpGet]
        [Route("recurringtransactions")]
        public async Task<ActionResult<IEnumerable<RecurringTransactionResponse>>> GetRecurringTransactions()
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            var transactions = await _mediatr.Send(new GetRecurringTransactionsQuery()
            {
                UserId = userId
            });
            return new OkObjectResult(transactions);
        }

        [HttpPost]
        [Route("recurringtransaction")]
        public async Task<ActionResult> AddRecurringTransaction([FromBody] RecurringTransactionRequest request)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            await _mediatr.Send(new AddRecurringTransactionQuery()
            {
                UserId = userId,
                Request = request
            });
            return new OkResult();
        }

        [HttpDelete]
        [Route("recurringtransaction/{id}")]
        public async Task<ActionResult> DeleteRecurringTransaction(string id)
        {
            var userId = GetUserIdFromCookie();
            if (userId is null)
            {
                return new UnauthorizedResult();
            }
            await _mediatr.Send(new DeleteRecurringTransactionCommand()
            {
                UserId = userId,
                Id = id
            });
            return new OkResult();
        }

        [HttpGet]
        [Route("categories/{partial}")]
        public async Task<ActionResult<IEnumerable<string>>> GetLedgerEntryCategories([FromBody] string partial) =>
            new OkObjectResult(await _mediatr.Send(new GetCategoriesQuery() { Partial = partial }));

        [HttpGet]
        [Route("frequencies")]
        public async Task<ActionResult<IEnumerable<Frequency>>> GetFrequencies() => new OkObjectResult(await _mediatr.Send(new GetLedgerItemsQuery<Frequency>()));

        [HttpGet]
        [Route("salarytypes")]
        public async Task<ActionResult<IEnumerable<SalaryType>>> GetSalaryTypes() => new OkObjectResult(await _mediatr.Send(new GetLedgerItemsQuery<SalaryType>()));

        [HttpGet]
        [Route("transactiontypes")]
        public async Task<ActionResult<IEnumerable<TransactionType>>> GetTransactionTypes() => new OkObjectResult(await _mediatr.Send(new GetLedgerItemsQuery<TransactionType>()));

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
