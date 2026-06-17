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
        private readonly IGenericRepository<Member> genericRepository;
        private readonly IGenericRepository<Membership> membershipRepository;
        private readonly IGenericRepository<Plan> planRepository;
        private readonly IGenericRepository<HealthRecord> healthRecordRepository;
        private readonly IGenericRepository<Booking> bookingRepository;

        public MemberService(IGenericRepository<Member> genericRepository ,
            IGenericRepository<Membership> membershipRepository,
            IGenericRepository<Plan> planRepository , 
            IGenericRepository<HealthRecord> healthRecordRepository , 
            IGenericRepository<Booking> BookingRepository)
        {
            this.genericRepository = genericRepository;
            this.membershipRepository = membershipRepository;
            this.planRepository = planRepository;
            this.healthRecordRepository = healthRecordRepository;
            bookingRepository = BookingRepository;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            //Check Email
            var emailExists = await genericRepository.AnyAsync(x => x.Email == model.Email, ct);
            //Check Phone number
            var phoneExists = await genericRepository.AnyAsync(x => x.Phone == model.Phone, ct);
            //Email Or Phone Exists return false
            if (phoneExists || emailExists) return false;
            //else return true add member
            var member = new Member()
            {
                Name = model.Name ,
                Email = model.Email ,
                Phone = model.Phone ,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street
                },
                HealthRecord = new HealthRecord()
                {
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Weight = model.HealthRecordViewModel.Weight,
                    Height = model.HealthRecordViewModel.Height,
                    Note = model.HealthRecordViewModel.Note
                }
            };
            var res = await genericRepository.AddAsync(member);
            return res > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await genericRepository.GetAllAsync(ct: ct);
            if (!members.Any()) return [];
            IEnumerable<MemberViewModel> result = members.Select(m => new MemberViewModel
            {
                Name = m.Name,
                Photo = m.Photo,
                Email = m.Email,
                Gender = m.Gender.ToString(),
                Phone = m.Phone ,
                Id = m.Id 
            });
            return result;
        }

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await genericRepository.GetByIdAsync(MemberId, ct);
            if (member == null) return null;
            var model = new MemberViewModel()
            {
                Name = member.Name,
                Phone = member.Phone,
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Gender = member.Gender.ToString(),
                Address = $"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City}",

            };
            var ActiveMembership = await membershipRepository.FirstOrDefaultAsync(x => x.MemberId ==  MemberId && x.EndDate > DateTime.Now);
            if(ActiveMembership is not null)
            {
                var ActivePlan = await planRepository.GetByIdAsync(ActiveMembership.PlanId, ct);
                model.PlanName = ActivePlan?.Name;
                model.MembershipStartDate = ActiveMembership.StartDate.ToString();
                model.MembershipEndDate = ActiveMembership.EndDate.ToString();
            }
            return model;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int MemberId, CancellationToken ct = default)
        {
            var record = await healthRecordRepository.FirstOrDefaultAsync(x => x.MemberId == MemberId, ct: ct);
            if (record == null) return null;
            return new HealthRecordViewModel()
            {
                Weight = record.Weight,
                Height = record.Height,
                BloodType = record.BloodType,
                Note = record.Note

            };
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await genericRepository.GetByIdAsync(MemberId, ct);
            if (member == null) return null;
            return new MemberToUpdateViewModel()
            {
                Name = member.Name,
                Phone = member.Phone,
                Email = member.Email,
                BuildingNumber = member.Address.BuildingNumber,
                Street = member.Address.Street,
                City = member.Address.City,
                Photo = member.Photo
            };
        }

        public async Task<bool> RemoveMemberAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await genericRepository.GetByIdAsync(MemberId, ct);
            if(member == null) return false;

            var HasFutureBookings = await bookingRepository.AnyAsync(x => x.MemberId == MemberId && x.Session.StartDate > DateTime.Now, ct);
            if(HasFutureBookings) return false;
            var res = await genericRepository.DeleteAsync(member, ct);
            return res > 0;
        }

        public async Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await genericRepository.GetByIdAsync(id, ct);
            if (member == null) return false;
            var emailExists = await genericRepository.AnyAsync(x => x.Email == model.Email && x.Id != id, ct); 
            var phoneExists = await genericRepository.AnyAsync(x => x.Phone == model.Phone && x.Id != id, ct);
            if(emailExists || phoneExists) return false;
            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.Street = model.Street;
            member.Address.City = model.City;
            member.UpdatedAt = DateTime.Now;

            var res = await genericRepository.UpdateAsync(member);
            return res > 0;
        }
    }
}
