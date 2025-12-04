using DmsSystem.Application.Interfaces;

using DmsSystem.Infrastructure.FileGeneration;
using DmsSystem.Infrastructure.Persistence.Contexts;
using DmsSystem.Infrastructure.Persistence.Repositories;
using DmsSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Text; // 記得加入 using

// 註冊額外的編碼支援 (例如 BIG5)
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);



var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DmsDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddScoped<IShmtParRepository, ShmtParRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<ICompanyInfoUploadService, CompanyInfoUploadService>();
builder.Services.AddScoped<IShmtSource4Repository, ShmtSource4Repository>();
builder.Services.AddScoped<IShareholderMeetingDetailService, ShareholderMeetingDetailService>();
builder.Services.AddScoped<IShmtSource1Repository, ShmtSource1Repository>();
builder.Services.AddScoped<IStockBalanceUploadService, StockBalanceUploadService>();
builder.Services.AddScoped<IStockBalanceRepository, StockBalanceRepository>();
builder.Services.AddScoped<IEquityRepository, EquityRepository>();
builder.Services.AddScoped<IShareholderReportRepository, ShareholderReportDapperRepository>();
builder.Services.AddScoped<IExcelGenerator, NpoiExcelGenerator>();

// 使用完整路徑，明確告訴編譯器要綁定哪兩個
//builder.Services.AddScoped<DmsSystem.Application.Interfaces.IReportService, DmsSystem.Infrastructure.Services.ReportService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
