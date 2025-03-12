using Cache;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.Handlers;
using ProductService.Application.Interfaces;
using ProductService.Application.Mapping;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Kafka;
using ProductService.Infrastructure.Repositories;
using Shared.Logging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// DI Kay�tlar�: EF Core ve MediatR

builder.Services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
// Repository'yi cache decorator ile sarmalama
builder.Services.Decorate<IProductRepository, CachedProductRepository>();
// Kafka publisher ile yay�nlama kayd�.
builder.Services.AddScoped<IEventPublisher, KafkaEventPublisher>();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddMediatR(typeof(GetAllProductsQueryHandler).Assembly);

// Shared.Cache �zerinden Redis cache servisini ekleme
builder.Services.AddRedisCache(builder.Configuration);

// Ortak loglama yap�land�rmas�.
builder.Services.AddSharedLogging(builder.Configuration);

// Di�er DI kay�tlar�: Controller, Swagger vb.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();