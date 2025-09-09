using StudentAutomation.Core.Abstractions;
namespace StudentAutomation.Application.DTOs.Users
{
    public class UserOperationClaimCreateDto : IDto
    {
        public int UserId { get; set; }
        public int OperationClaimId { get; set; }
    }
}
