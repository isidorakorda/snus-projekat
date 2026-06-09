using k8s;
using Microsoft.EntityFrameworkCore;
using SensorRegistry.BgServices;
using SensorRegistry.Controllers;
using SensorRegistry.Data;
using SensorRegistry.Services;
using SensorRegistry.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

bool isRunningInK8s = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST"));
Console.WriteLine("[SensorRegistry] CONF: " + builder.Configuration.GetConnectionString("DefaultConnection"));


builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (isRunningInK8s)
{
    var config = KubernetesClientConfiguration.InClusterConfig();
    builder.Services.AddSingleton<IKubernetes>(new Kubernetes(config));
    builder.Services.AddHostedService<SensorStatusWorker>();
    Console.WriteLine("SensorRegistry Running in K8s: Kubernetes and Background Services enabled.");
}
else
{
    builder.Services.AddSingleton<IKubernetes?>(sp => null);
    Console.WriteLine("[SensorRegistry] Running locally: K8s and Background Services disabled.");
}

builder.Services.AddScoped<IK8Service, K8Service>();
builder.Services.AddScoped<ISensorLifecycleService, SensorLifecycleService>();
builder.Services.AddHostedService<SensorStatusWorker>();


builder.Services.AddDbContext<SensorDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
