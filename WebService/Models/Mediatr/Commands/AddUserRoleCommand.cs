using MediatR;

namespace WebService
{
    public class AddUserRoleCommand : IRequest
    {
        public string CreatedBy { get; set; }
        public string Description { get; set; }
    }
}