using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Treasure.Data;
using Treasure.Helpers.Appsetting;
using Treasure.Helpers.Mapper;
using Treasure.Helpers.Middlewares;
using Treasure.Models.Treasuare;
using Treasure.Services;
using Treasure.Validator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true) // allow any origin
            .AllowCredentials(); // allow credentials
    });
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddDbContext<AppDBContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"))
);

// Add DI
builder.Services.AddScoped<ITreasureService, TreasureService>();
builder.Services.AddScoped<IValidator<MatrixRequest>, TreasureRequestValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//cache
builder.Services.AddMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    var rd = builder.Configuration.GetSection("Redis").Get<RedisSetting>();
    options.Configuration = rd.sConn;
    options.InstanceName = rd.InstanceName;
});
builder.Services.Add(ServiceDescriptor.Singleton<IDistributedCache, RedisCache>());

// AutoMapper configuration
var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperProfile());
});
var mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
