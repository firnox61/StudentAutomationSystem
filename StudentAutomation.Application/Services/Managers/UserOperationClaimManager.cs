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
    public class UserOperationClaimManager : IUserOperationClaimService
    {
        private readonly IUserOperationClaimDal _userOperationClaimDal;
        private readonly IMapper _mapper;

        public UserOperationClaimManager(IUserOperationClaimDal userOperationClaimDal, IMapper mapper)
        {
            _userOperationClaimDal = userOperationClaimDal;
            _mapper = mapper;
        }

        [ValidationAspect(typeof(UserOperationClaimCreateDtoValidator))]
        [TransactionScopeAspect]
        [CacheRemoveAspect("IUserOperationClaimService.Get*")]
        public async Task<IResult> AddAsync(UserOperationClaimCreateDto dto)
        {
            var entity = _mapper.Map<UserOperationClaim>(dto);
            await _userOperationClaimDal.AddAsync(entity);
            return new SuccessResult("Kullanıcıya rol atandı.");
        }
        [TransactionScopeAspect]
        [CacheRemoveAspect("IUserOperationClaimService.Get*")]
        public async Task<IResult> DeleteAsync(int id)
        {
            var entity = await _userOperationClaimDal.GetAsync(x => x.Id == id);
            if (entity == null)
                return new ErrorResult("Rol ataması bulunamadı.");

            await _userOperationClaimDal.DeleteAsync(entity);
            return new SuccessResult("Rol ataması silindi.");
        }
        [CacheAspect]
        public async Task<IDataResult<List<UserOperationClaimListDto>>> GetByUserIdAsync(int userId)
        {
            var list = await _userOperationClaimDal.GetWithDetailsByUserIdAsync(userId);

            var dtoList = list.Select(x => new UserOperationClaimListDto
            {
                Id = x.Id,
                UserFullName = $"{x.User.FirstName} {x.User.LastName}",
                RoleName = x.OperationClaim.Name
            }).ToList();

            return new SuccessDataResult<List<UserOperationClaimListDto>>(dtoList);
        }
    }
}
