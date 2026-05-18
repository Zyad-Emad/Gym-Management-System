using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Models
{
    public class Member : GymUser 
    {
        public string? Photo { get; set; }
        //JoinDate = CreatedAt of BaseEntity
        public HealthRecord HealthRecord { get; set; } = default!;
        public DateTime JoinDate { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
        public ICollection<Membership> Memberships { get; set; } = new HashSet<Membership>();
    }
}
