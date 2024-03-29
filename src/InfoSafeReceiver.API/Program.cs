using InfoSafeReceiver.API.Configurations;
using InfoSafeReceiver.API.Messaging;
using InfoSafeReceiver.API.Messaging.ExtraForLearning;
using InfoSafeReceiver.API.Services;
using InfoSafeReceiver.Application;
using InfoSafeReceiver.Data;
using InfoSafeReceiver.Data.Repositories;
using InfoSafeReceiver.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry(option =>
        option.ConnectionString = builder.Configuration.GetConnectionString("ApplicationInsightsConnectionString")
    );

// Add services to the container.
builder.Services.AddDbContext<InfoSafeReceiverDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnectionString"))
            );
builder.Services.AddAutoMapperConfiguration();
builder.Services.AddScoped<MessagingService>();
builder.Services.AddScoped<IAppService, AppService>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<RmqFanOutServiceBusConsumer>();
}
else
{
    builder.Services.AddHostedService<AzServiceBusConsumer>();
}

builder.Services.AddHangfireConfiguration(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<InfoSafeReceiverDbContext>();
    dataContext.Database.Migrate();
}

app.ApplyHangfire();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();