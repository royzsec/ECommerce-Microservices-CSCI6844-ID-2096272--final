using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlite("Data Source=orders.db"));

builder.Services.AddHttpClient<ICustomerClient, CustomerClient>(client =>
{
    client.BaseAddress = new Uri("http://customerservice:8080/");
});

builder.Services.AddHttpClient<IProductClient, ProductClient>(client =>
{
    client.BaseAddress = new Uri("http://productservice:8080/");
});

builder.Services.AddSingleton<RabbitMQPublisher>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
