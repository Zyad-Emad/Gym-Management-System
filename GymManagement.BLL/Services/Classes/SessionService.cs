using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Data.Models.Enums;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;

        public SessionService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate) return Result.ValidationFailed("EndDate Must be After Start Date") ;
            if (model.StartDate <= DateTime.Now) return Result.ValidationFailed("Start Date Must Be In The Future");
            if (model.Capacity < 1 || model.Capacity > 25) return Result.ValidationFailed("Capacity Must Be Between 1 and 25");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
            if (trainer is null) return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId, ct);
            if (category is null) return Result.NotFound("Category Not Found");

            var isValid = Enum.TryParse<Speciality>(category.Name , true , out var CategorySpeciality);
            if(!isValid || trainer.Speciality != CategorySpeciality) return Result.ValidationFailed("Trainer Speciality Does Not Match Session Category");

            var session = mapper.Map<CreateSessionViewModel, Session>(model);

            _unitOfWork.GetRepository<Session>().Add(session);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.OK() : Result.Fail("Failed to create session");
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.SessionRepository.GetSessionsWithTrainerAndCategoryAsync(ct: ct);    

            var mappedSessions = mapper.Map<IEnumerable<Session>, IEnumerable<SessionViewModel>>(sessions).ToList();
  
            foreach(var session in mappedSessions)
            {
                session.AvailableSlots =  session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
                //N + 1 Problem
            }
            return mappedSessions;
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            var result = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct: ct);
            return mapper.Map<IEnumerable<Category>, IEnumerable<CategorySelectViewModel>>(result);
        }

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int id, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategoryByIdAsync(id, ct);
            if (session is null) return Result<SessionViewModel>.NotFound("Session Not Found");
            var mappedSession = mapper.Map<Session, SessionViewModel>(session);
            var AvailableSlots = mappedSession.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);
            return Result<SessionViewModel>.OK(mappedSession);
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int id, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct); 
            if(session is null)return Result<UpdateSessionViewModel>.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now) return Result<UpdateSessionViewModel>.Fail("Cannot Update Session That Has Already Started Or Ended");

            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);

            if (bookingCount > 0) return Result<UpdateSessionViewModel>.Fail("Cannot Update Session That Has Bookings");

            var mappedSession = mapper.Map<Session, UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.OK(mappedSession);
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            var result = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return mapper.Map<IEnumerable<Trainer>, IEnumerable<TrainerSelectViewModel>>(result);
        }

        public async Task<Result> RemoveSessionAsync(int id, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
            if (session is null) return Result.NotFound("Session Not Found");

            if (session.EndDate >= DateTime.Now)
                return Result.Fail("Cannot Remove Session That Has Not Ended Yet");
            
            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);
            if (bookingCount > 0) return Result.Fail("Cannot Remove Session That Has Bookings");

            _unitOfWork.SessionRepository.Delete(session);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.OK() : Result.Fail("Failed to remove session");
        }

        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
            if (session is null) return Result.NotFound("Session Not Found");
            
            if (session.StartDate <= DateTime.Now) return Result.Fail("Cannot Update Session That Has Already Started Or Ended");

            if (model.EndDate <= model.StartDate) return Result.ValidationFailed("EndDate Must be After Start Date");

            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);

            if (bookingCount > 0) return Result.Fail("Cannot Update Session That Has Bookings");

            if (model.StartDate <= DateTime.Now) return Result.ValidationFailed("Start Date Must Be In The Future");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
            if (trainer is null) return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId, ct);

            var isValid = Enum.TryParse<Speciality>(category?.Name, true, out var CategorySpeciality);
            if (!isValid || trainer.Speciality != CategorySpeciality) return Result.ValidationFailed("Trainer Speciality Does Not Match Session Category");

            mapper.Map(model, session);
            session.UpdatedAt = DateTime.Now;
            _unitOfWork.SessionRepository.Update(session);

            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.OK() : Result.Fail("Failed to update session");
        }
    }
}

