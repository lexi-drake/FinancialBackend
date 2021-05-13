using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using WebService;

namespace Tests
{
    public static class MockExtensions
    {
        public static void SetupUserRepo(this Mock<IUserRepository> repo, string invalidUserId)
        {
            IEnumerable<User> users = new List<User>() { new User() };
            IEnumerable<User> noUsers = new List<User>();

            repo.Setup(x => x.GetUsersByIdAsync(It.IsAny<string>()))
                .Returns<string>(id =>
                {
                    var response = id == invalidUserId ? noUsers : users;
                    return Task.FromResult(response);
                });
        }
    }
}