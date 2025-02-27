using Microsoft.EntityFrameworkCore;

namespace P01_2022_SG_650_2022_PM_650.Models
{
    public class ReservasContext : DbContext
    {
        public ReservasContext(DbContextOptions<ReservasContext> options) : base(options){ }
    }
}
