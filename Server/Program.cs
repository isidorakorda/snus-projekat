using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Service;
using Server.Service.IService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

string alarmHubUrl = Environment.GetEnvironmentVariable("ALARM_HUB_URL") ?? "https://localhost:5052/alarmHub";
builder.Services.AddSingleton<IAlarmPublisher>(sp => new AlarmPublisher(alarmHubUrl));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ServerDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

bool hasMigrated = false;

while (!hasMigrated)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ServerDbContext>();
            Console.WriteLine("[Server] Checking and applying database migrations...");
            context.Database.Migrate();
            hasMigrated = true;
            Console.WriteLine("[Server] Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Server] Failed to apply migrations: {ex.Message}");
            Thread.Sleep(3000);
        }
    }
}



app.UseSwagger();
app.UseSwaggerUI();


//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
