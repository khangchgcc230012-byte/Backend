using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// Fix for PostgreSQL timestamp issues
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Database Configuration
builder.Services.AddDbContext<WebApplication1Context>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WebApplication1Context") ??
    throw new InvalidOperationException("Connection string not found.")));

// 1. CORS Setup
builder.Services.AddCors(options => {
    options.AddPolicy("AllowVercel",
        policy => {
            policy.WithOrigins("https://frontend-woad-eight-25.vercel.app")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. IMPORTANT: Use the exact policy name defined above
app.UseCors("AllowVercel");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();