using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.Handlers;
using ProductService.Application.Interfaces;
using ProductService.Application.Mapping;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// DI Kayýtlarý: EF Core ve MediatR

builder.Services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddMediatR(typeof(GetAllProductsQueryHandler).Assembly);

// Diðer DI kayýtlarý: Controller, Swagger vb.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();