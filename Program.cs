using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllers();  // Registrar controladores
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro de servicios personalizados (conexión a SSAS y servicio de datos)
builder.Services.AddSingleton<CubeConnection>();  // Servicio de conexión al cubo
builder.Services.AddScoped<CubeDataService>();    // Servicio de consultas al cubo

var app = builder.Build();

// Configuración del pipeline de la aplicación
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Mapear controladores a rutas
app.MapControllers();

app.Run();

