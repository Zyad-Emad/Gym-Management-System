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

        public MemberService(IGenericRepository<Member> genericRepository)
        {
            this.genericRepository = genericRepository;
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
    }
}
