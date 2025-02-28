using Microsoft.EntityFrameworkCore;

namespace P01_2022_SG_650_2022_PM_650.Models
{
    public class ReservasContext : DbContext
    {
        public ReservasContext(DbContextOptions<ReservasContext> options) : base(options)
        { }

        public DbSet<Usuario> usuario { get; set; }
        public DbSet<Sucursal> sucursal { get; set; }
        public DbSet<Reserva> reserva  { get; set; }
        public DbSet<EspaciosParqueo> espaciosParqueo { get; set; }
    }
}
