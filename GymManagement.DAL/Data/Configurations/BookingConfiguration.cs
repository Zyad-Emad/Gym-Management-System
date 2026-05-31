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
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasOne(b => b.Member)
                   .WithMany(m => m.Bookings) 
                   .HasForeignKey(b => b.MemberId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(b => b.Attended)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(b => b.Date)
                   .HasDefaultValueSql("GETDATE()");

            builder.HasIndex(b => new
            {
                b.MemberId,
                b.SessionId
            }).IsUnique();


        }
    }
}
