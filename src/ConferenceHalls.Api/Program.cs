using ConferenceHalls.Api.Data;
using ConferenceHalls.Api.Middleware;
using ConferenceHalls.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Без ключа доступа приложение не запускаем
var apiKey = builder.Configuration["ApiKeys:Key"];
if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new InvalidOperationException("ApiKeys:Key is not configured");
}

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Подключаем сервисы с бизнес-логикой
builder.Services.AddScoped<IHallService, HallService>();
builder.Services.AddScoped<IAdditionalServiceService, AdditionalServiceService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddControllers();

// Единая обработка ошибок
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks();

// Swagger: документация API и кнопка для ввода ключа
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Conference Halls API",
        Version = "v1"
    });

    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = ApiKeyMiddleware.HeaderName
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("ApiKey", document)] = new List<string>()
    });
});

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // При старте создаём таблицы и заполняем начальными данными
    db.Database.Migrate();
    await DbSeeder.SeedAsync(db, app.Environment.IsDevelopment());
}

app.UseSwagger();
app.UseSwaggerUI();

// Проверка ключа стоит после Swagger, чтобы Swagger открывался без ключа
app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
