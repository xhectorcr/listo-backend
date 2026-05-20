using System.Text;
using ListoWeb.API.Data; // Importar el Inicializador
using ListoAPI.Aplication.Core.Interfaces;
using ListoAPI.Aplication.Infrastructure.Data;
using ListoAPI.Aplication.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Robustez para cargar variables de entorno en Render/Linux
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    jwtKey = Environment.GetEnvironmentVariable("Jwt__Key") 
             ?? Environment.GetEnvironmentVariable("JWT_KEY")
             ?? Environment.GetEnvironmentVariable("Jwt_Key")
             ?? "DefaultSecretKeyForListoAPI2026!!!";
    builder.Configuration["Jwt:Key"] = jwtKey;
}

var connString = builder.Configuration.GetConnectionString("ListoBD");
if (string.IsNullOrEmpty(connString))
{
    connString = Environment.GetEnvironmentVariable("ConnectionStrings__ListoBD")
                 ?? Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__LISTOBD")
                 ?? Environment.GetEnvironmentVariable("ConnectionStrings_ListoBD");
    if (!string.IsNullOrEmpty(connString))
    {
        builder.Configuration["ConnectionStrings:ListoBD"] = connString;
    }
}

var emailPass = builder.Configuration["EmailSettings:Password"];
if (string.IsNullOrEmpty(emailPass))
{
    emailPass = Environment.GetEnvironmentVariable("EmailSettings__Password")
                ?? Environment.GetEnvironmentVariable("EmailSettings_Password")
                ?? Environment.GetEnvironmentVariable("EMAILSETTINGS__PASSWORD");
    if (!string.IsNullOrEmpty(emailPass))
    {
        builder.Configuration["EmailSettings:Password"] = emailPass;
    }
}

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ConfigContext>(options =>
{

    options.UseNpgsql(
        builder.Configuration.GetConnectionString("ListoBD"), 
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null); 
        });
        
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging(); 
});
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IMetodoPagoRepository, MetodoPagoRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll"); // ¡Añade esta línea!

// Script de Inicialización de Base de Datos
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ConfigContext>();
    try 
    {
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ocurrió un error inicializando la BD: {ex.Message}");
    }
}





if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
