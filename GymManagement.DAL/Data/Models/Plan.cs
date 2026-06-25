namespace GymManagement.DAL.Data.Models
{
    public class Plan : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Membership> Memberships { get; set; } = new HashSet<Membership>();
    }
}
