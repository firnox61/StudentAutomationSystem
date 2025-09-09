using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAutomation.Application.DTOs.Users;
using StudentAutomation.Application.Interfaces.Services.Contracts;

namespace StudentAutomation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationClaimsController : ControllerBase
    {
        private readonly IOperationClaimService _operationClaimService;

        public OperationClaimsController(IOperationClaimService operationClaimService)
        {
            _operationClaimService = operationClaimService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _operationClaimService.GetAllAsync();
            return result.Success ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _operationClaimService.GetByIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(result.Message);
        }

        [HttpPost]
        public async Task<IActionResult> Add(OperationClaimCreateDto dto)
        {
            var result = await _operationClaimService.AddAsync(dto);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPut]
        public async Task<IActionResult> Update(OperationClaimUpdateDto dto)
        {
            var result = await _operationClaimService.UpdateAsync(dto);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _operationClaimService.DeleteAsync(id);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }
    }
}
