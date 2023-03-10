using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Services;
using Services.Contracts;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
    config.CacheProfiles.Add("5mins", new CacheProfile() { Duration = 300 });
})
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly)
    //.AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters()
    .AddCustomCsvFormat();


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//IoC
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureLoggerService();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureActionFilters();
builder.Services.ConfigureCors();
builder.Services.ConfigureDataShaper();
builder.Services.AddCustomMediaTypes();
builder.Services.AddScoped<IBookLinks, BookLinks>();
builder.Services.ConfigureVersioning();
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

//Middlewares
var logger = app.Services.GetRequiredService<ILoggerService>();
app.ConfigureExceptionHAndler(logger);


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseIpRateLimiting(); //IpRateLimiting should be call before Cors
app.UseCors("CorsPolicy");
app.UseResponseCaching(); // ResponseCaching should be call after Cors
app.UseHttpCacheHeaders();

app.UseAuthorization();

app.MapControllers();

app.Run();
