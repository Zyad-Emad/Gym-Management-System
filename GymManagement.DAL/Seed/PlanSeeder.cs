using GymManagement.DbContexts;
using GymManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Seed
{
    public static class PlanSeeder
    {
        public static async Task SeedAsync(GymDbContext context)
        {
            if (!context.Plans.Any())
            {
                var plans = new List<Plan>
                {
                    new Plan
                    {
                        Name = "Basic Plan",
                        Description = "Access to gym floor and cardio area during standard hours.",
                        DurationDays = 30,
                        Price = 300.00m,
                        IsActive = true
                    },
                    new Plan
                    {
                        Name = "Premium Plan",
                        Description = "Full access to gym floor, group classes, and free locker access.",
                        DurationDays = 180,
                        Price = 1500.00m,
                        IsActive = true
                    },
                    new Plan
                    {
                        Name = "VIP Annual Plan",
                        Description = "All-inclusive access plus 5 personal training sessions per month.",
                        DurationDays = 365,
                        Price = 2500.00m,
                        IsActive = true
                    }
                };

                await context.Plans.AddRangeAsync(plans);
                await context.SaveChangesAsync();
            }
        }
    }
}
