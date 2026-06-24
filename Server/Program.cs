using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Service;
using Server.Service.IService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddSingleton<IAlarmPublisher>(sp => new AlarmPublisher("https://localhost:5052/alarmHub"));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ServerDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
