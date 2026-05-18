using GymManagement.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Models
{
    public class HealthRecord : BaseEntity
    {

        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string? Note { get; set; }
        public BloodType BloodType { get; set; }
        //LastUpdated = UpdatedAt of BaseEntity
        public int MemberId { get; set; }
        public Member Member { get; set; } = default!;
    }
}
