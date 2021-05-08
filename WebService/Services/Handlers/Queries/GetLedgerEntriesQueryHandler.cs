using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class GetLedgerEntriesQueryHandler : IRequestHandler<GetLedgerEntriesQuery, IEnumerable<LedgerEntryResponse>>
    {
        private ILogger _logger;
        private ILedgerRepository _repo;

        public GetLedgerEntriesQueryHandler(ILogger logger, ILedgerRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<IEnumerable<LedgerEntryResponse>> Handle(GetLedgerEntriesQuery query, CancellationToken cancellation)
        {
            var startDate = FromMilliseconds(query.Start);
            var endDate = FromMilliseconds(query.End);
            if (startDate == DateTime.MinValue || endDate == DateTime.MinValue || endDate < startDate)
            {
                _logger.Throw($"Unable to parse millesecond values {query.Start} and/or {query.End}.");
            }
            var transactionTypes = await _repo.GetAllAsync<TransactionType>();
            return from entry in await _repo.GetLedgerEntriesBetweenDatesAsync(startDate, endDate, query.UserId)
                   select LedgerEntryResponse.FromDBObject(entry, transactionTypes);
        }

        private DateTime FromMilliseconds(string milliseconds)
        {
            var ticks = double.Parse(milliseconds);
            var timespan = TimeSpan.FromMilliseconds(ticks);
            return new DateTime(1970, 1, 1) + timespan;
        }
    }
}