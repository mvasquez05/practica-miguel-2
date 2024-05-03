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

// Agregar servicios para la l�gica de negocio
builder.Services.AddScoped<AlertaService>();
builder.Services.AddScoped<CargaDatosService>();

// Agregar servicios para MVC y configurar NewtonsoftJson como el serializador por defecto.
// A�adir soporte para XML.
builder.Services.AddControllers()
    .AddXmlSerializerFormatters()
    .AddNewtonsoftJson()
    .AddMvcOptions(options =>
    {
        options.MaxValidationDepth = 200; // Aumenta este valor seg�n sea necesario
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
        Title = "API Datos Meteorol�gicos",
        Description = "Prueba Avanzada para Desarrollador 3TECH"
    });

    

    //options.AddConsumesConstraintForApi("application/xml");
    options.SupportNonNullableReferenceTypes();
});

var app = builder.Build();

// Middleware para servir la documentaci�n generada como un JSON de Swagger
app.UseSwagger();

// Middleware para servir la UI de Swagger en la ruta ra�z
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Datos Meteorol�gicos V1");
    options.RoutePrefix = string.Empty;
});

// Configurar el pipeline de solicitudes HTTP
app.UseHttpsRedirection();


// A�adir el middleware de enrutamiento
app.UseRouting();

app.UseCors(); // Aseg�rate de llamar a UseCors()

app.UseAuthorization();

// Configurar el pipeline de solicitudes HTTP con endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Asegura que los controladores est�n registrados
});

app.Run();
