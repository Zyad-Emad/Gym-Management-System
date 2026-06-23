using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Attachment;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;
        private readonly IAttachmentService attachmentService;

        public MemberService(IUnitOfWork unitOfWork , IMapper mapper , IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.attachmentService = attachmentService;
        }

        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            //Check Email
            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email, ct);
            //Check Phone number
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone, ct);
            //Email Or Phone Exists return false
            if (phoneExists) return Result.ValidationFailed("The Same Phone Number Already Exists");
            if (emailExists) return Result.ValidationFailed("The Same Email Already Exists");

            //Upload Photo
            var result = await attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "MembersPhoto");
            if (string.IsNullOrWhiteSpace(result.data)) return Result.Fail(result.error!);
            
            //else return true add member
            var member = mapper.Map<CreateMemberViewModel, Member>(model);
            member.Photo = result.data;
            _unitOfWork.GetRepository<Member>().Add(member);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            if (res > 0) return Result.OK();
            //delete uploaded photo
            attachmentService.Delete(result.data, "MembersPhoto");
            return Result.Fail("Failed to create member");
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any()) return [];
            IEnumerable<MemberViewModel> result = mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members);
            return result;
        }

        public async Task<Result<MemberViewModel>> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);
            if (member == null) return Result<MemberViewModel>.NotFound("Member Not Found");
            var model = mapper.Map<Member , MemberViewModel>(member);
            var ActiveMembership = await _unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(x => x.MemberId ==  MemberId && x.EndDate > DateTime.Now);
            if(ActiveMembership is not null)
            {
                var ActivePlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(ActiveMembership.PlanId, ct);
                model.PlanName = ActivePlan?.Name;
                model.MembershipStartDate = ActiveMembership.StartDate.ToString();
                model.MembershipEndDate = ActiveMembership.EndDate.ToString();
            }
            return Result<MemberViewModel>.OK(model);
        }

        public async Task<Result<HealthRecordViewModel>> GetMemberHealthRecordAsync(int MemberId, CancellationToken ct = default)
        {
            var record = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(x => x.MemberId == MemberId, ct: ct);
            if (record == null) return Result<HealthRecordViewModel>.NotFound("Health Record Not Found");
            var res = mapper.Map<HealthRecord, HealthRecordViewModel>(record);
            return  Result<HealthRecordViewModel>.OK(res);
        }

        public async Task<Result<MemberToUpdateViewModel>> GetMemberToUpdateAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);
            if (member == null) return Result<MemberToUpdateViewModel>.NotFound("Member Not Found");
            var res = mapper.Map<Member, MemberToUpdateViewModel>(member);
            return Result<MemberToUpdateViewModel>.OK(res);
        }

        public async Task<Result> RemoveMemberAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);
            if(member == null) return Result.NotFound("Member Not Found");

            var HasFutureBookings = await _unitOfWork.GetRepository<Booking>().AnyAsync(x => x.MemberId == MemberId && x.Session.StartDate > DateTime.Now, ct);
            if(HasFutureBookings) return Result.ValidationFailed("Member cannot be deleted because of Future bookings");
            _unitOfWork.GetRepository<Member>().Delete(member);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            if (res > 0)
            {
                if (!string.IsNullOrWhiteSpace(member.Photo))
                {
                    attachmentService.Delete(member.Photo, "MembersPhoto");
                }
                return Result.OK();
            }
            return Result.Fail("Failed To Remove Member");
        }

        public async Task<Result> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return Result.NotFound("Member Not Found");
            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email && x.Id != id, ct); 
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone && x.Id != id, ct);

            if (phoneExists) return Result.ValidationFailed("The Same Phone Number Already Exists");
            if (emailExists) return Result.ValidationFailed("The Same Email Already Exists");

            mapper.Map<MemberToUpdateViewModel, Member>(model, member);
            member.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().Update(member);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.OK() : Result.Fail("Failed To Update Member");
        }
    }
}
