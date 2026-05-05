using ListoAPI.Aplication.Core.Entities;
using ListoAPI.Aplication.Core.Interfaces;
using ListoAPI.Aplication.Infrastructure.Data;
using ListoAPI.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;



namespace ListoAPI.Aplication.Infrastructure.Repository
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



        public async Task<ResponseCommonDTO> saveItem(UsuarioDTO pItem)
        {
            try
            {

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(pItem.Password);

                // Buscar el IdRol del Cliente si no se proporcionó uno
                int? rolAsignar = pItem.IdRol;
                if (rolAsignar == null || rolAsignar == 0)
                {
                    var rolCliente = await _context.ROL.FirstOrDefaultAsync(r => r.Nombre == "Cliente");
                    if (rolCliente != null)
                    {
                        rolAsignar = rolCliente.IdRol;
                    }
                }

                var user = new Usuario
                {
                    Password = passwordHash,
                    Estado = pItem.Estado,
                    Nombre = pItem.Nombre,
                    Correo = pItem.Correo,
                    Telefono = pItem.Telefono,
                    IdRol = rolAsignar
                };

                await _context.USUARIO.AddAsync(user);
                await _context.SaveChangesAsync();
                return new ResponseCommonDTO { success = true, message = "Usuario guardado correctamente" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en saveItem (Usuario): {ex.Message}");
                return new ResponseCommonDTO { success = false, message = "Error interno al crear el usuario." };
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

                // 5. LEER LA CONFIGURACIÓN Y CREAR LA FIRMA
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                var expiracion = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"]));

                // 6. ENSAMBLAR EL TOKEN
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

                // 7. retonar el json con el token
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
                return new ResponseCommonDTO { success = false, message = "Error interno al validar el acceso." };
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

        public Task<List<UsuarioDTO>> getList(string pSearch = "")
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