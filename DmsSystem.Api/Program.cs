using DmsSystem.Api.Middleware;
using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using DmsSystem.Application.Services;
using DmsSystem.Domain.Entities;
using DmsSystem.Infrastructure.FileGeneration;
using DmsSystem.Infrastructure.FileParsing;
using DmsSystem.Infrastructure.Persistence.Contexts;
using DmsSystem.Infrastructure.Persistence.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text;

// 註冊編碼提供者，支援 Big5 編碼（用於 CSV 檔案讀取）
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// 設定 Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/dms-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("應用程式啟動中...");

    var builder = WebApplication.CreateBuilder(args);

    // 使用 Serilog 取代預設的日誌提供者
    builder.Host.UseSerilog();

    // ============================================
    // 資料庫配置
    // ============================================
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("資料庫連接字串未設定。請檢查 appsettings.json 或環境變數。");
    }

    builder.Services.AddDbContext<DmsDbContext>(options =>
        options.UseSqlServer(connectionString));

    // ============================================
    // 依賴注入設定
    // ============================================

    // Repository 註冊
    builder.Services.AddScoped<IShmtParRepository, ShmtParRepository>();
    builder.Services.AddScoped<IContractRepository, ContractRepository>();
    builder.Services.AddScoped<IShmtSource4Repository, ShmtSource4Repository>();
    builder.Services.AddScoped<IShmtSource1Repository, ShmtSource1Repository>();
    builder.Services.AddScoped<IStockBalanceRepository, StockBalanceRepository>();
    builder.Services.AddScoped<IEquityRepository, EquityRepository>();
    builder.Services.AddScoped<IShareholderReportRepository, ShareholderReportDapperRepository>();

    // 檔案解析器註冊
    builder.Services.AddScoped<IFileParser<ShmtSource1>, ShmtSource1FileParser>();
    builder.Services.AddScoped<IFileParser<ShmtSource4>, ShmtSource4FileParser>();
    builder.Services.AddScoped<IFileParser<StockBalanceCsvRecord>, StockBalanceCsvParser>();

    // Service 註冊（現在在 Application 層）
    builder.Services.AddScoped<ICompanyInfoUploadService, CompanyInfoUploadService>();
    builder.Services.AddScoped<IShareholderMeetingDetailService, ShareholderMeetingDetailService>();
    builder.Services.AddScoped<IStockBalanceUploadService, StockBalanceUploadService>();
    builder.Services.AddScoped<IReportService, ReportService>();

    // 檔案產生器
    builder.Services.AddScoped<IExcelGenerator, NpoiExcelGenerator>();

    // FluentValidation
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

    // ============================================
    // API 設定
    // ============================================
    builder.Services.AddControllers();

    // Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // CORS 設定（供 React 前端使用）
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000",      // React 開發伺服器預設埠
                    "http://localhost:5173",     // Vite 開發伺服器預設埠
                    "http://localhost:8080"       // 其他可能的開發埠
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    var app = builder.Build();

    // ============================================
    // HTTP 請求管道設定
    // ============================================

    // 全域例外處理（必須在最前面）
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    // Swagger（僅開發環境）
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // CORS（必須在 UseAuthorization 之前）
    app.UseCors("AllowReactApp");

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    Log.Information("應用程式啟動完成");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "應用程式啟動失敗");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
