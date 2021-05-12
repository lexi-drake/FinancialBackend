using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using MediatR;
using WebService;

namespace Tests
{
    public static class HandlerExtensions
    {
        public static async Task AssertThrowsArgumentExceptionWithMessage<T, U>(this IRequestHandler<T, U> handler, T query, string message) where T : IRequest<U>
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(query, new CancellationToken()));
            Assert.Equal(message, ex.Message);
        }
    }
}