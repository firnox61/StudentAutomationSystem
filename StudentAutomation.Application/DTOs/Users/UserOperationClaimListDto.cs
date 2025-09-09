using StudentAutomation.Core.Abstractions;
namespace StudentAutomation.Application.DTOs.Users
{
    public class UserOperationClaimListDto : IDto
    {
        public int Id { get; set; }
        public string UserFullName { get; set; }
        public string RoleName { get; set; }
    }
}
