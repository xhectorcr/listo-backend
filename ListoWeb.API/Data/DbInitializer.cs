using ListoAPI.Aplication.Core.Entities;
using ListoAPI.Aplication.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ListoWeb.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ConfigContext context)
        {
            // Asegurarse de que la base de datos y las tablas existan
            context.Database.EnsureCreated();

            // Asegurarse de que la columna 'dni' exista en la tabla 'Usuario'
            try
            {
                context.Database.ExecuteSqlRaw("ALTER TABLE \"Usuario\" ADD COLUMN IF NOT EXISTS \"dni\" text;");
                Console.WriteLine("--> Columna 'dni' verificada/agregada a la tabla 'Usuario'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Nota: No se pudo agregar la columna 'dni' automáticamente: {ex.Message}");
            }

            // Crear tabla de Historial de Compras si no existe
            try
            {
                context.Database.ExecuteSqlRaw(@"
                    CREATE TABLE IF NOT EXISTS ""HistorialCompra"" (
                        ""id_historial"" SERIAL PRIMARY KEY,
                        ""id_usuario"" INT NOT NULL,
                        ""fecha"" TIMESTAMP NOT NULL,
                        ""total"" NUMERIC NOT NULL,
                        ""cantidad_items"" INT NOT NULL
                    );
                ");
                Console.WriteLine("--> Tabla 'HistorialCompra' verificada/creada.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Nota: No se pudo crear la tabla 'HistorialCompra' automáticamente: {ex.Message}");
            }

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
