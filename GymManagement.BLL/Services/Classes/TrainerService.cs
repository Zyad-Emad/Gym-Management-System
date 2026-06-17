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
        private readonly IGenericRepository<Trainer> trainerRepository;
        private readonly IGenericRepository<Session> sessionRepository;

        public TrainerService(IGenericRepository<Trainer> trainerRepository , 
            IGenericRepository<Session> sessionRepository)
        {
            this.trainerRepository = trainerRepository;
            this.sessionRepository = sessionRepository;
        }
        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            if(await trainerRepository.AnyAsync(t => t.Email == model.Email || t.Phone == model.Phone, ct))
            {
                return false;
            }
            var trainer = new Trainer()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Speciality = model.Specialities,
                DateOfBirth = model.DateOfBirth,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street,
                    City = model.City
                }
            };
            var res = await trainerRepository.AddAsync(trainer, ct);
            return res > 0;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await trainerRepository.GetAllAsync(ct: ct);
            return trainers.Select(t => new TrainerViewModel()
            {
                Id = t.Id,
                Name = t.Name,
                Email = t.Email,
                Phone = t.Phone,
                Specialities = t.Speciality.ToString()
            });
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int TrainerId, CancellationToken ct = default)
        {
            var trainer = await trainerRepository.GetByIdAsync(TrainerId, ct);
            if (trainer == null) return null;
            return new TrainerViewModel()
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Specialities = trainer.Speciality.ToString(),
                DateOfBirth = trainer.DateOfBirth.ToShortDateString(),
                Address = $"{trainer.Address.BuildingNumber} - {trainer.Address.Street} - {trainer.Address.City}",

            };  
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int TrainerId, CancellationToken ct = default)
        {
            var trainer = await trainerRepository.GetByIdAsync(TrainerId, ct);
            if(trainer == null) return null;
            return new TrainerToUpdateViewModel()
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Specialities = trainer.Speciality,
                BuildingNumber = trainer.Address.BuildingNumber,
                Street = trainer.Address.Street,
                City = trainer.Address.City
            };
        }

        public async Task<bool> RemoveTrainerAsync(int TrainerId, CancellationToken ct = default)
        {
            var trainer = await trainerRepository.GetByIdAsync(TrainerId, ct);
            if (trainer == null) return false;
            var hasFutureSessions = await sessionRepository.AnyAsync(s => s.TrainerId == TrainerId && 
            s.StartDate > DateTime.Now, ct);  
            if(hasFutureSessions) return false;
            var res = await trainerRepository.DeleteAsync(trainer, ct);
            return res > 0;
        }

        public async Task<bool> UpdateTrainerDetailsAsync(int TrainerId, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await trainerRepository.GetByIdAsync(TrainerId, ct);
            if (trainer == null) return false;
            if (await trainerRepository.AnyAsync(t => (t.Email == model.Email || t.Phone == model.Phone) && t.Id != TrainerId, ct))
                return false;
            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Speciality = model.Specialities;
            trainer.Address = new Address()
            {
                BuildingNumber = model.BuildingNumber,
                Street = model.Street,
                City = model.City
            };
            trainer.UpdatedAt = DateTime.Now;
            var res = await trainerRepository.UpdateAsync(trainer, ct);
            return res > 0;
        }
    }
}
