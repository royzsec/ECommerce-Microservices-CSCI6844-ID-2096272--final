using Microsoft.EntityFrameworkCore;
using CustomerService.Data;
using CustomerService.Models;

var builder = WebApplication.CreateBuilder(args);

var dbPath = "/app/data/customers.db";
builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    context.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
