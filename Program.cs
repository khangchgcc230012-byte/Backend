using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// Ensure Npgsql can handle local timestamps if necessary, 
// though using DateTime.UtcNow in your code is the preferred professional approach.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
builder.Services.AddDbContext<WebApplication1Context>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WebApplication1Context") ??
    throw new InvalidOperationException("Connection string 'WebApplication1Context' not found.")));

// 1. Define the CORS policy to allow your Vercel Frontend to access this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVercelApp",
        policy =>
        {
            policy.WithOrigins("https://frontend-woad-eight-25.vercel.app/") // Replace with your actual Vercel URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. Enable CORS - This must be placed after UseRouting (implicit here) and before UseAuthorization
app.UseCors("AllowVercelApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();