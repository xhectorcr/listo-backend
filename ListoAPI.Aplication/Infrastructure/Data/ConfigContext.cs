using ListoAPI.Aplication.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ListoAPI.Aplication.Infrastructure.Data
{
    public class ConfigContext : DbContext
    {
        public ConfigContext(DbContextOptions<ConfigContext> options) : base(options)
        {
        }
 
        public DbSet<Rol> ROL { get; set; }
        public DbSet<Usuario> USUARIO { get; set; }

        public DbSet<Categoria> CATEGORIA { get; set; }

        public DbSet<Producto> PRODUCTO {get;set;}
       
    }
    
}