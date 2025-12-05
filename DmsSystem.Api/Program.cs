using DmsSystem.Application.Interfaces;
using DmsSystem.Infrastructure.FileGeneration;
using DmsSystem.Infrastructure.Persistence.Contexts;
using DmsSystem.Infrastructure.Persistence.Repositories;
using DmsSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

// 註冊編碼提供者，支援 Big5 編碼（用於 CSV 檔案讀取）
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 資料庫配置
// ============================================
// 資料庫連接字串配置
// 優先順序：環境變數 > appsettings.{Environment}.json > appsettings.json
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

// Service 註冊
// 【注意】目前 Service 實作在 Infrastructure 層，建議移至 Application 層
builder.Services.AddScoped<ICompanyInfoUploadService, CompanyInfoUploadService>();
builder.Services.AddScoped<IShareholderMeetingDetailService, ShareholderMeetingDetailService>();
builder.Services.AddScoped<IStockBalanceUploadService, StockBalanceUploadService>();

// 檔案產生器
builder.Services.AddScoped<IExcelGenerator, NpoiExcelGenerator>();

// 【註解】ReportService 待實作
//builder.Services.AddScoped<IReportService, ReportService>();

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

app.Run();
