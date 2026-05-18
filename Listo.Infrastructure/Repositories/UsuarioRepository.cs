using Listo.Domain.Entities;
using Listo.Application.Interfaces;
using Listo.Infrastructure.Data;
using Listo.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;



namespace Listo.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        protected readonly ConfigContext _context;
        private readonly IConfiguration _configuration;

        public UsuarioRepository(ConfigContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }


        public async Task<(List<UsuarioDTO> usuarios, int TotalCount)> GetUsuarioList(
    int pageNumber = 1,
    int pageSize = 10,
    string pSearch = "",
    int idRol = 0
)
        {
            try
            {
                pSearch = pSearch?.Trim().ToLower();
                var query = from u in _context.USUARIO
                            join r in _context.ROL on u.IdRol equals r.IdRol
                            where u.Estado == true 
                            select new UsuarioDTO
                            {
                                IDUsuario = u.IdUsuario, 
                                Estado = u.Estado,
                                Nombre = u.Nombre,
                                Correo = u.Correo,
                                Telefono = u.Telefono,
                                IdRol = u.IdRol,
                                Rol = r.Nombre 
                                                     
                            };

                if (idRol > 0)
                {
                    query = query.Where(x => x.IdRol == idRol);
                }
                if (!string.IsNullOrEmpty(pSearch))
                {
                    query = query.Where(x => x.Nombre.ToLower().Contains(pSearch) ||
                                             x.Correo.ToLower().Contains(pSearch));
                }

                var totalCount = await query.CountAsync();
                var usuarios = await query
                    .OrderBy(u => u.Nombre)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (usuarios, totalCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetUsuarioList: {ex.Message}");
                return (new List<UsuarioDTO>(), 0);
            }
        }


        public async Task<ResponseCommonDTO> saveItem(UsuarioDTO pItem)
        {
            try
            {

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(pItem.Password);

                var user = new Usuario
                {
                    Password = passwordHash,
                    Estado = pItem.Estado,
                    Nombre = pItem.Nombre,
                    Correo = pItem.Correo,
                    Telefono = pItem.Telefono,
                    IdRol = pItem.IdRol
                };

                await _context.USUARIO.AddAsync(user);
                await _context.SaveChangesAsync();
                return new ResponseCommonDTO { success = true, message = "Usuario guardado correctamente" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en saveItem (Usuario): {ex.Message}");
                return new ResponseCommonDTO { success = false, message = $"Error interno al crear el usuario: {ex.Message}" };
            }
        }

        public async Task<ResponseCommonDTO> RegisterClientAsync(RegistroClienteDTO pItem)
        {
            try
            {

                if (pItem == null)
                {
                    return new ResponseCommonDTO { success = false, message = "Los datos enviados son inválidos." };
                }

                bool correoExiste = await _context.USUARIO.AnyAsync(u => u.Correo == pItem.Correo);
                if (correoExiste)
                {
                    return new ResponseCommonDTO { success = false, message = "El correo ya se encuentra registrado." };
                }

                if (!string.IsNullOrWhiteSpace(pItem.Dni))
                {
                    bool dniExiste = await _context.USUARIO.AnyAsync(u => u.Dni == pItem.Dni);
                    if (dniExiste)
                    {
                        return new ResponseCommonDTO { success = false, message = "El DNI ya se encuentra registrado." };
                    }
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(pItem.Password);

                var user = new Usuario
                {
                    Nombre = pItem.Nombre,
                    Dni = pItem.Dni,
                    Correo = pItem.Correo,
                    Telefono = pItem.Telefono,
                    Password = passwordHash,
                    IdRol = 2,
                    Estado = true
                };

                await _context.USUARIO.AddAsync(user);
                await _context.SaveChangesAsync();

                return new ResponseCommonDTO { success = true, message = "Cliente registrado correctamente." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en RegisterClientAsync: {ex.Message}");
                return new ResponseCommonDTO { success = false, message = $"Error interno al crear el cliente: {ex.Message}" };
            }
        }




        public async Task<ResponseCommonDTO> ValidarLogin(string correo, string password)
        {
            try
            {

                var query = await (from u in _context.USUARIO
                                   join r in _context.ROL on u.IdRol equals r.IdRol
                                   where u.Correo == correo
                                   select new
                                   {
                                       Usuario = u,
                                       NombreRol = r.Nombre
                                   }).FirstOrDefaultAsync();


                if (query == null)
                {
                    return new ResponseCommonDTO { success = false, message = "Credenciales incorrectas." };
                }

                if (!query.Usuario.Estado)
                {
                    return new ResponseCommonDTO { success = false, message = "Su cuenta se encuentra inactiva." };
                }

                bool esPasswordValido = BCrypt.Net.BCrypt.Verify(password, query.Usuario.Password);

                if (!esPasswordValido)
                {
                    return new ResponseCommonDTO { success = false, message = "Credenciales incorrectas." };
                }

                var claims = new[]
                {
            new Claim("idUsuario", query.Usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, query.Usuario.Nombre),
            new Claim(ClaimTypes.Email, query.Usuario.Correo),
            new Claim(ClaimTypes.Role, query.NombreRol) // Aquí usamos el JOIN
        };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                var expiracion = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"]));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = expiracion,
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = credenciales
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                string tokenString = tokenHandler.WriteToken(token);

                return new ResponseCommonDTO
                {
                    success = true,
                    message = "Bienvenido al sistema",
                    data = new
                    {
                        token = tokenString,
                        usuario = query.Usuario.Nombre,
                        rol = query.NombreRol
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ValidarLogin: {ex.Message}");
                return new ResponseCommonDTO { success = false, message = $"Error interno al validar el acceso: {ex.Message}" };
            }
        }


        public Task<ResponseCommonDTO> deleteItem(int pId, int idUsuarioInt, string ipOrigen)
        {
            throw new NotImplementedException();
        }

        public Task<UsuarioDTO> getById(int pId)
        {
            throw new NotImplementedException();
        }

        public Task<List<UsuarioDTO>> getInactivosList(string pSearch = "")
        {
            throw new NotImplementedException();
        }

        public Task<ResponseCommonDTO> RecuperarUsuario(int pId)
        {
            throw new NotImplementedException();
        }
        public Task<ResponseCommonDTO> updateItem(UsuarioDTO pItem)
        {
            throw new NotImplementedException();
        }


    }

}
