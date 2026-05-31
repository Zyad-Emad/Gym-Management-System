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
    internal class GymUserConfiguration<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasDiscriminator<string>("UserType");

            builder.Property(x => x.Name)
                   .HasColumnType("varchar")
                   .HasMaxLength(50);

            builder.Property(x => x.Email)
                   .HasColumnType("varchar")
                   .HasMaxLength(100);

            builder.Property(x => x.Phone)
                   .HasColumnType("varchar")
                   .HasMaxLength(20);

            builder.OwnsOne(x => x.Address, t =>
            {
                t.Property(a => a.Street)
                .HasColumnName("Street")
                .HasMaxLength(100);
                t.Property(a => a.City)
                .HasColumnName("City")
                .HasMaxLength(100);
                t.Property(a => a.BuildingNumber)
                .HasColumnName("BuildingNumber");
            });

            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.Phone).IsUnique();

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("EmailCheck", "Email like '_%@_%._%'");
                tb.HasCheckConstraint("PhoneCheck", "LEN(Phone) = 11 AND Phone Like '01[0125]%'");
            });


        }
    }
}
