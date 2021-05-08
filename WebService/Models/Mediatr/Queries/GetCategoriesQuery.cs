using System.Collections.Generic;
using MediatR;

namespace WebService
{
    public class GetCategoriesQuery : IRequest<IEnumerable<string>>
    {
        public string Partial { get; set; }
    }
}