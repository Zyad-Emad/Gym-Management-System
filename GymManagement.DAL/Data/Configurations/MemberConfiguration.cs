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
    internal class MemberConfiguration : GymUserConfiguration<Member>
    {
        public override void Configure(EntityTypeBuilder<Member> builder)
        {
            base.Configure(builder);

            //Member configuration
            builder.Property(m => m.JoinDate)
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(m => m.Photo)
                   .HasMaxLength(500);

        }
    }
}
