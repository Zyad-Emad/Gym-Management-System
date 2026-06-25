using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.Configurations
{
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("DateCheck", "EndDate > StartDate");
            });

            builder.Property(ms => ms.StartDate)
                   .HasDefaultValueSql("GETDATE()");

            builder.HasOne(ms => ms.Member)
                   .WithMany(m => m.Memberships)
                   .HasForeignKey(ms => ms.MemberId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(ms => ms.Plan)
                   .WithMany(p => p.Memberships)
                   .HasForeignKey(ms => ms.PlanId)
                   .OnDelete(DeleteBehavior.Restrict);
            //builder.HasIndex(ms => new
            //{
            //    ms.MemberId,
            //    ms.PlanId
            //}).IsUnique();
        }
    }
}
