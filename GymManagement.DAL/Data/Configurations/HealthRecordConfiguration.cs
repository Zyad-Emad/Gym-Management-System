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
    internal class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
    {
        public void Configure(EntityTypeBuilder<HealthRecord> builder)
        {
            builder.Property(hr => hr.Height)
                   .HasPrecision(5, 2);
            builder.Property(hr => hr.Weight)
                   .HasPrecision(5, 2);
            builder.Property(hr => hr.BloodType)
                   .HasConversion<string>()
                   .HasMaxLength(50);
            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("HeightCheck", "Height > 0");
                tb.HasCheckConstraint("WeightCheck", "Weight > 0");

            });
        }
    }
}
