using Backend.Data;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Necesario para Swagger UI
builder.Services.AddSwaggerGen();

// PostgreSQL (usa DefaultConnection en appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Servicios propios
builder.Services.AddScoped<FolioService>();

// Registrar WhatsAppService para inyección de dependencias
builder.Services.AddSingleton(new WhatsAppService(
    builder.Configuration["WhatsApp:AccessToken"],
    builder.Configuration["WhatsApp:PhoneNumberId"]
));

var app = builder.Build();

// Middleware|
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
