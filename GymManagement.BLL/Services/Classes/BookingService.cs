using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.BookingViewModels;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class BookingService :IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null) return Result.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now) return Result.Fail("Cannot Cancel a booking for a session that already started");
            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.SessionId == sessionId && b.MemberId == memberId , true , ct);

            if (booking is null) return Result.NotFound("Booking Not Found");

            _unitOfWork.BookingRepository.Delete(booking);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.OK() : Result.Fail("Booking Cancel Failed");
        }

        public async Task<Result> CreateNewBookingAsync(CreateBookingViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(model.SessionId, ct);
            if (session is null) return Result.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now) return Result.Fail("Cannot Book A session that has already started");

            var hasActiveMembership = await _unitOfWork.MembershipRepository.AnyAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now , ct);

            if (!hasActiveMembership) return Result.Fail("Member does not have an active membership");

            var alreadyBooked = await _unitOfWork.BookingRepository.AnyAsync(b => b.SessionId == model.SessionId && b.MemberId == model.MemberId , ct);

            if (alreadyBooked)
                return Result.Fail("Member is Already booked for this session");

            var booked = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(model.SessionId, ct);
            if (booked >= session.Capacity) return Result.Fail("Session is full");

            _unitOfWork.BookingRepository.Add(new Booking
            {
                MemberId = model.MemberId,
                SessionId = model.SessionId,
                Attended = false , 
                CreatedAt = DateTime.Now
            });
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.OK() : Result.Fail("Failed to book session");
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.SessionRepository.GetSessionsWithTrainerAndCategoryAsync(x => x.EndDate >= DateTime.Now , ct);

            if (!bookings.Any()) return null!;

            var MappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(bookings);

            foreach(var item in MappedSessions)
            {
                item.AvailableSlots = item.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(item.Id, ct);

            }
            return MappedSessions;
        }

        public async Task<IEnumerable<MemberSelectListViewModel>> GetMemberForDropDownAsync(int sessionId, CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetAllAsync(x => x.SessionId == sessionId , ct : ct );

            var bookedMembersIds = bookings.Select(x => x.MemberId);

            var availableMembers = await _unitOfWork.GetRepository<Member>().GetAllAsync(x => !bookedMembersIds.Contains(x.Id));

            return _mapper.Map<IEnumerable<MemberSelectListViewModel>>(availableMembers);
        }

        public async Task<IEnumerable<MemberForSessionViewModel>> GetMemberForOnGoingBySessionIdAsync(int sessionId, CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBookingBySessionIdAsync(sessionId , ct : ct );
            return bookings.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.MemberId,
                SessionId = b.SessionId,
                MemberName = b.Member.Name,
                BookingDate = b.CreatedAt.ToShortDateString()
            }).ToList();
        }

        public async Task<IEnumerable<MemberForSessionViewModel>> GetMemberForUpComingBySessionIdAsync(int sessionId, CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBookingBySessionIdAsync(sessionId, ct: ct);
            return bookings.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.MemberId,
                SessionId = b.SessionId,
                MemberName = b.Member.Name,
                BookingDate = b.CreatedAt.ToShortDateString(),
                IsAttended = b.Attended
            }).ToList();
        }

        public async Task<Result> MarkAttendedAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId , true , ct);

            if (booking is null) return Result.NotFound("Booking not found");

            booking.Attended = true;
            booking.UpdatedAt = DateTime.Now;

            _unitOfWork.BookingRepository.Update(booking);

            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.OK() : Result.Fail("Failed to Mark as attended");
        }
    }
}
