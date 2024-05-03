using APIDatosMeteorologicos.Data;
using APIDatosMeteorologicos.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Agregar el DbContext
builder.Services.AddDbContext<APIDatosMeteorologicosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar servicios para la lógica de negocio
builder.Services.AddScoped<AlertaService>();
builder.Services.AddScoped<CargaDatosService>();

// Agregar servicios para MVC y configurar NewtonsoftJson como el serializador por defecto.
// Añadir soporte para XML.
builder.Services.AddControllers()
    .AddXmlSerializerFormatters()
    .AddNewtonsoftJson()
    .AddMvcOptions(options =>
    {
        options.MaxValidationDepth = 200; // Aumenta este valor según sea necesario
    });


// Configurar CORS para permitir todas las peticiones. Necesario para enviar XML desde Swagger al endpoint correspondiente

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API Datos Meteorológicos",
        Description = "Prueba Avanzada para Desarrollador 3TECH"
    });

    

    //options.AddConsumesConstraintForApi("application/xml");
    options.SupportNonNullableReferenceTypes();
});

var app = builder.Build();

// Middleware para servir la documentación generada como un JSON de Swagger
app.UseSwagger();

// Middleware para servir la UI de Swagger en la ruta raíz
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Datos Meteorológicos V1");
    options.RoutePrefix = string.Empty;
});

// Configurar el pipeline de solicitudes HTTP
app.UseHttpsRedirection();


// Añadir el middleware de enrutamiento
app.UseRouting();

app.UseCors(); // Asegúrate de llamar a UseCors()

app.UseAuthorization();

// Configurar el pipeline de solicitudes HTTP con endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Asegura que los controladores están registrados
});

app.Run();
