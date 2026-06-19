using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.Seed
{
    public static class GymDataSeeding
    {
        public static async Task SeedAsync(GymDbContext dbContext , string seedFolderPath , ILogger logger,  CancellationToken ct = default) 
        {
            try
            {
                if ( !await dbContext.Plans.AnyAsync(ct))
                {
                    var Plans = LoadDataFromJsonFile<Plan>(seedFolderPath, "plans.Json");
                    if (Plans.Any())
                    {
                        dbContext.Plans.AddRange(Plans);
                        logger.LogInformation($"Plans Seeded with Count = {Plans.Count}");
                    }
                    if (dbContext.ChangeTracker.HasChanges())
                    {
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        logger.LogInformation("Plan Already Seeded");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Gym Data Seeding Failed");
                throw;
            }

        }
        private static List<T> LoadDataFromJsonFile<T>(string folderPath  , string fileName)
        {
            var filePath = Path.Combine(folderPath, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Seed Data File Not Found : {filePath}");

            var data = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];

        }
    }
}
