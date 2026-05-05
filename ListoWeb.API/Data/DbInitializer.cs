using ListoAPI.Aplication.Core.Entities;
using ListoAPI.Aplication.Infrastructure.Data;

namespace ListoWeb.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ConfigContext context)
        {
            // Asegurarse de que la base de datos y las tablas existan
            context.Database.EnsureCreated();

            // 1. Verificar e insertar Roles si no existen
            if (!context.ROL.Any())
            {
                var roles = new[]
                {
                    new Rol { Nombre = "Administrador" },
                    new Rol { Nombre = "Cliente" }
                };
                
                context.ROL.AddRange(roles);
                context.SaveChanges();
                Console.WriteLine("--> Roles por defecto creados.");
            }

            // 2. Verificar e insertar la cuenta de Administrador por defecto
            bool adminExists = context.USUARIO.Any(u => u.Correo == "admin@listo.com");
            if (!adminExists)
            {
                var adminRol = context.ROL.FirstOrDefault(r => r.Nombre == "Administrador");
                if (adminRol != null)
                {
                    var adminUser = new Usuario
                    {
                        Nombre = "Admin Listo",
                        Correo = "admin@listo.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                        Telefono = "000000000",
                        Estado = true,
                        IdRol = adminRol.IdRol
                    };
                    
                    context.USUARIO.Add(adminUser);
                    context.SaveChanges();
                    Console.WriteLine("--> Cuenta de administrador por defecto (admin@listo.com) creada.");
                }
            }
            else
            {
                Console.WriteLine("--> La cuenta de administrador ya existe. Conservando datos actuales.");
            }
        }
    }
}
