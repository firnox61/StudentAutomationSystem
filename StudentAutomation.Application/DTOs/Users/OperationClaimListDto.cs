using StudentAutomation.Core.Abstractions;
namespace StudentAutomation.Application.DTOs.Users
{
    public class OperationClaimListDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
