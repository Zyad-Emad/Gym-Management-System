using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembershipService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result> CreateMembershipAsync(CreateMembershipViewModel model, CancellationToken ct = default)
        {
            var memberExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Id == model.MemberId , ct);
            if (!memberExists) return Result.NotFound("Member Not Found");
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(model.PlanId , ct);
            
            if (plan is null) return Result.NotFound("Plan Not Found");
            
            if (!plan.IsActive) return Result.Fail("Plan Is Not Active");
            
            var hasActive = await _unitOfWork.MembershipRepository.AnyAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now, ct);

            if (hasActive) return Result.Fail("Member Already has an Active Membership");

            var entity = new Membership
            {
                MemberId = model.MemberId,
                PlanId = model.PlanId,
                CreatedAt = DateTime.Now,
                EndDate = (model.StartDate ?? DateTime.Now).AddDays(plan.DurationDays)
            };
            _unitOfWork.MembershipRepository.Add(entity);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.OK() : Result.Fail("Failed to Create New Membership");
        }

        public async Task<Result> DeleteActiveMembershipAsync(int memberId, CancellationToken ct = default)
        {
            var active = await _unitOfWork.MembershipRepository
                .FirstOrDefaultAsync(m => m.MemberId == memberId && m.EndDate > DateTime.Now, true ,  ct);

            if (active is null) return Result.NotFound("No Active membership for this member");

            _unitOfWork.MembershipRepository.Delete(active);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.OK() : Result.Fail("Failed to Delete This membership");
        }

        public async Task<IEnumerable<MembershipViewModel>> GetAllMembershipAsync(CancellationToken ct = default)
        {
            var memberships = await _unitOfWork.MembershipRepository.GetAllMemershipsWithMembersAndPlansAsync(m => m.EndDate > DateTime.Now, ct);

            return _mapper.Map<IEnumerable<Membership> , IEnumerable<MembershipViewModel>>(memberships);
        }

        public async Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropDownListAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct : ct);
            return _mapper.Map<IEnumerable<Member>, IEnumerable<MemberSelectListViewModel>>(members);
        }

        public async Task<IEnumerable<PlanSelectListViewModel>> GetPlansForDropDownListAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct : ct);
            return _mapper.Map<IEnumerable<Plan>, IEnumerable<PlanSelectListViewModel>>(plans);
        }
    }
}
