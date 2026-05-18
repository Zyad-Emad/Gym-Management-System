using GymManagement.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Models
{
    public class Trainer : GymUser
    {
        //HireDate = CreatedAt of BaseEntity
        public Speciality Speciality { get; set; }
        public DateTime HireDate { get; set; }
        public ICollection<Session> Sessions { get; set; } = new HashSet<Session>();
    }
}
