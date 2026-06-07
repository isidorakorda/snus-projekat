using k8s;
using Microsoft.EntityFrameworkCore;
using SensorRegistry.BgServices;
using SensorRegistry.Controllers;
using SensorRegistry.Data;
using SensorRegistry.Services;
using SensorRegistry.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var config = KubernetesClientConfiguration.InClusterConfig();
Console.WriteLine(">>> CONN: " + builder.Configuration.GetConnectionString("DefaultConnection"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IKubernetes>(new Kubernetes(config));

builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddHostedService<SensorStatusWorker>();

builder.Services.AddDbContext<SensorDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
