using AutoMapper;
using StudentAutomation.Application.DTOs.Users;
using StudentAutomation.Application.Interfaces.Services.Contracts;
using StudentAutomation.Application.Repositories;
using StudentAutomation.Application.Validators.Users;
using StudentAutomation.Core.Aspects.Caching;
using StudentAutomation.Core.Aspects.Transaction;
using StudentAutomation.Core.Aspects.Validation;
using StudentAutomation.Core.Utilities.Results;
using StudentAutomation.Domain.Entities;


namespace StudentAutomation.Application.Services.Managers
{
    public class OperationClaimManager : IOperationClaimService
    {
        private readonly IOperationClaimDal _operationClaimDal;
        private readonly IMapper _mapper;

        public OperationClaimManager(IOperationClaimDal operationClaimDal, IMapper mapper)
        {
            _operationClaimDal = operationClaimDal;
            _mapper = mapper;
        }
        [CacheAspect]
        public async Task<IDataResult<List<OperationClaimListDto>>> GetAllAsync()
        {
            var entities = await _operationClaimDal.GetAllAsync();
            var dtoList = _mapper.Map<List<OperationClaimListDto>>(entities);
            return new SuccessDataResult<List<OperationClaimListDto>>(dtoList);
        }
        [CacheAspect]
        public async Task<IDataResult<OperationClaimListDto>> GetByIdAsync(int id)
        {
            var entity = await _operationClaimDal.GetAsync(o => o.Id == id);
            if (entity == null)
                return new ErrorDataResult<OperationClaimListDto>("Rol bulunamadı");

            var dto = _mapper.Map<OperationClaimListDto>(entity);
            return new SuccessDataResult<OperationClaimListDto>(dto);
        }
        [TransactionScopeAspect]
        [ValidationAspect(typeof(OperationClaimCreateDtoValidator))]
        [CacheRemoveAspect("IOperationClaimService.Get*")]
        public async Task<IResult> AddAsync(OperationClaimCreateDto dto)
        {
            var entity = _mapper.Map<OperationClaim>(dto);
            await _operationClaimDal.AddAsync(entity);
            return new SuccessResult("Rol eklendi");
        }
        [TransactionScopeAspect]
        [ValidationAspect(typeof(OperationClaimUpdateDtoValidator))]
        [CacheRemoveAspect("IOperationClaimService.Get*")]
        public async Task<IResult> UpdateAsync(OperationClaimUpdateDto dto)
        {
            var entity = await _operationClaimDal.GetAsync(x => x.Id == dto.Id);
            if (entity == null)
                return new ErrorResult("Güncellenecek rol bulunamadı");

            entity.Name = dto.Name;
            await _operationClaimDal.UpdateAsync(entity);
            return new SuccessResult("Rol güncellendi");
        }
        [TransactionScopeAspect]
        [CacheRemoveAspect("IOperationClaimService.Get*")]
        public async Task<IResult> DeleteAsync(int id)
        {
            var entity = await _operationClaimDal.GetAsync(x => x.Id == id);
            if (entity == null)
                return new ErrorResult("Silinecek rol bulunamadı");

            await _operationClaimDal.DeleteAsync(entity);
            return new SuccessResult("Rol silindi");
        }
    }

}