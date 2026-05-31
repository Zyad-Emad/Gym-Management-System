using GymManagement.DAL.Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAllAsync(GymDbContext context)
        {
            await CategorySeeder.SeedAsync(context);
            await PlanSeeder.SeedAsync(context);

        }
    }
}
