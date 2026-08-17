using Faturamento.API.Integrations;
using Faturamento.API.Middlewares;
using Faturamento.API.Validators;
using Faturamento.Application.Exceptions;
using Faturamento.Application.Interfaces;
using Faturamento.Application.Services;
using Faturamento.Domain.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Faturamento.Infrastructure.Persistence;
using Faturamento.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CriarNotaFiscalRequestDtoValidator>();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("FaturamentoDb")
    ?? throw new InvalidOperationException("Connection string 'FaturamentoDb' não foi configurada.");

builder.Services.AddDbContext<FaturamentoDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.Configure<EstoqueIntegrationOptions>(
    builder.Configuration.GetSection(EstoqueIntegrationOptions.SectionName));

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

builder.Services
    .AddHttpClient<IEstoqueIntegrationService, EstoqueIntegrationService>((sp, client) =>
    {
        var options = sp.GetRequiredService<IConfiguration>()
            .GetSection(EstoqueIntegrationOptions.SectionName)
            .Get<EstoqueIntegrationOptions>();

        if (options is null || string.IsNullOrWhiteSpace(options.BaseUrl))
            throw new EstoqueIntegrationException("Não foi possível comunicar com o Estoque.");

        client.BaseAddress = new Uri(options.BaseUrl);
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddPolicyHandler(retryPolicy);

builder.Services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
builder.Services.AddScoped<IFaturamentoService, FaturamentoService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.MapControllers();

app.Run();
