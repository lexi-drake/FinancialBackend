using System.Collections.Generic;
using MediatR;

namespace WebService
{
    public class GetIncomeGeneratorsQuery : IRequest<IEnumerable<IncomeGeneratorResponse>>
    {
        public string UserId { get; set; }
    }
}