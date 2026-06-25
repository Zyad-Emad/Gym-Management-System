using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.MemberViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default);
        Task<Result> CreateMemberAsync(CreateMemberViewModel model , CancellationToken ct = default);
        Task<Result<MemberViewModel>> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default);
        Task<Result<HealthRecordViewModel>> GetMemberHealthRecordAsync(int MemberId, CancellationToken ct = default);
        Task<Result<MemberToUpdateViewModel>> GetMemberToUpdateAsync(int MemberId, CancellationToken ct = default);
        Task<Result> UpdateMemberDetailsAsync(int id , MemberToUpdateViewModel model , CancellationToken ct = default);
        Task<Result> RemoveMemberAsync(int MemberId, CancellationToken ct = default);
    }
}
