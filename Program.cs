global using PetHostelApi.Contexts;
global using Microsoft.EntityFrameworkCore;
using PetHostelApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configure CORS for mobile apps
builder.Services.AddCors(options =>
{
    options.AddPolicy("MobileAppPolicy",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<AuthService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("MobileAppPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Endpoints informativos
app.MapGet("/", () => Results.Ok("PetHostel API is running! 🐾"));

app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    database = "connected" // Podrías hacer un check real aquí
}));

app.Run();

