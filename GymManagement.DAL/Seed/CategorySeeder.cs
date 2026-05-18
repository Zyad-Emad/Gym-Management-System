using GymManagement.DAL.Models;
using GymManagement.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Seed
{
    public static class CategorySeeder
    {
        public static async Task SeedAsync(GymDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                new Category { Name = "Cardio" },
                new Category { Name = "Bodybuilding" },
                new Category { Name = "CrossFit" },
                new Category { Name = "Yoga" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
        }
    }
}
