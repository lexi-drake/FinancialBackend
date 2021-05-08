using System.Collections.Generic;
using MediatR;

namespace WebService
{
    public class AddIncomeGeneratorCommand : IRequest
    {
        public string UserId { get; set; }
        public IncomeGeneratorRequest Request { get; set; }
        public IEnumerable<string> TransactionIds { get; set; }
    }
}