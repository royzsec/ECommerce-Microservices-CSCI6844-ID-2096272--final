using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Services;

var builder = WebApplication.CreateBuilder(args);

var dbPath = "/app/data/payments.db";
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<PaymentConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
    context.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
