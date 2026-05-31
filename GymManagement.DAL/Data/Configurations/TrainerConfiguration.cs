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
    internal class TrainerConfiguration : GymUserConfiguration<Trainer>
    {
        public override void Configure(EntityTypeBuilder<Trainer> builder)
        {
            base.Configure(builder);

            //Trainer Configuration
            builder.Property(t => t.HireDate)
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(t => t.Speciality)
                .HasConversion<string>()
                .HasMaxLength(30);
        }
    }
}
