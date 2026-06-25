using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;

        public TrainerService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email , ct))
                return Result.ValidationFailed("The Same Email Already Exists");
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone , ct))
                return Result.ValidationFailed("The Same Phone Number Already Exists");
            var trainer = mapper.Map<CreateTrainerViewModel, Trainer>(model);
            _unitOfWork.GetRepository<Trainer>().Add(trainer);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.OK() : Result.Fail("Failed To Create");
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            var res = mapper.Map<IEnumerable<Trainer> , IEnumerable<TrainerViewModel>>(trainers);
            return res;
        }

        public async Task<Result<TrainerViewModel>> GetTrainerDetailsAsync(int TrainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId, ct);
            if (trainer == null) return Result<TrainerViewModel>.NotFound("Trainer Not Found");
            var res = mapper.Map<Trainer , TrainerViewModel>(trainer);
            return Result<TrainerViewModel>.OK(res);
        }

        public async Task<Result<TrainerToUpdateViewModel>> GetTrainerToUpdateAsync(int TrainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId, ct);
            if(trainer == null) return Result<TrainerToUpdateViewModel>.NotFound("Trainer Not Found");
            var res = mapper.Map<Trainer , TrainerToUpdateViewModel>(trainer);

            return Result<TrainerToUpdateViewModel>.OK(res);
        }

        public async Task<Result> RemoveTrainerAsync(int TrainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId, ct);
            if (trainer == null) return Result.NotFound("Trainer Not Found");
            var hasActiveOrFutureSessions =
            await _unitOfWork.GetRepository<Session>()
            .AnyAsync(s =>
            s.TrainerId == TrainerId &&
            s.EndDate > DateTime.Now,
            ct);

            if (hasActiveOrFutureSessions)
                return Result.ValidationFailed(
                    "Trainer has active or upcoming sessions.");
            _unitOfWork.GetRepository<Trainer>().Delete(trainer);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.OK() : Result.Fail("Failed To Remove Trainer");
        }

        public async Task<Result> UpdateTrainerDetailsAsync(int TrainerId, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(TrainerId, ct);
            if (trainer == null) return Result.NotFound("Trainer Not Found");
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email && t.Id != TrainerId, ct))
                return Result.ValidationFailed("The Same Email Already Exists");
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone && t.Id != TrainerId, ct))
                return Result.ValidationFailed("The Same Phone Number Already Exists");
            mapper.Map<TrainerToUpdateViewModel, Trainer>(model, trainer);
            trainer.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            var res = await _unitOfWork.SaveChangesAsync(ct);
            return res > 0 ? Result.OK() : Result.Fail("Update Failed");
        }
    }
}
