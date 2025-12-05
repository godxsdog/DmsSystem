using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using DmsSystem.Application.Services;
using DmsSystem.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DmsSystem.Tests.Services;

public class StockBalanceUploadServiceTests
{
    private readonly Mock<IStockBalanceRepository> _stockBalanceRepoMock;
    private readonly Mock<IContractRepository> _contractRepoMock;
    private readonly Mock<IEquityRepository> _equityRepoMock;
    private readonly Mock<IFileParser<StockBalanceCsvRecord>> _fileParserMock;
    private readonly Mock<ILogger<StockBalanceUploadService>> _loggerMock;
    private readonly StockBalanceUploadService _service;

    public StockBalanceUploadServiceTests()
    {
        _stockBalanceRepoMock = new Mock<IStockBalanceRepository>();
        _contractRepoMock = new Mock<IContractRepository>();
        _equityRepoMock = new Mock<IEquityRepository>();
        _fileParserMock = new Mock<IFileParser<StockBalanceCsvRecord>>();
        _loggerMock = new Mock<ILogger<StockBalanceUploadService>>();
        _service = new StockBalanceUploadService(
            _stockBalanceRepoMock.Object,
            _contractRepoMock.Object,
            _equityRepoMock.Object,
            _fileParserMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ProcessUploadAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var stream = new MemoryStream();
        var fileName = "stockbalance.csv";
        var csvRecords = new List<StockBalanceCsvRecord>
        {
            new StockBalanceCsvRecord
            {
                Pcode = "TT01",
                AcDate = "2024/1/1",
                Isin = "TW0001234567",
                Shares = 1000
            }
        };

        var contract = new Contract
        {
            Id = "TT1",
            ContractSeq = "001"
        };

        var equity = new Equity
        {
            StkCd = "1234",
            DealCtr = "TW"
        };

        _fileParserMock.Setup(x => x.ParseAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(csvRecords);
        _contractRepoMock.Setup(x => x.FindByPcodeAsync("TT1"))
            .ReturnsAsync(contract);
        _equityRepoMock.Setup(x => x.FindByIsinAsync("TW0001234567"))
            .ReturnsAsync(equity);
        _stockBalanceRepoMock.Setup(x => x.FindByKeyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<string>()))
            .ReturnsAsync((StockBalance?)null);
        _stockBalanceRepoMock.Setup(x => x.AddAsync(It.IsAny<StockBalance>()))
            .Returns(Task.CompletedTask);
        _stockBalanceRepoMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.ProcessUploadAsync(stream, fileName);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, result.RowsAdded);
        Assert.Equal(0, result.RowsUpdated);
        Assert.Equal(0, result.RowsFailed);
        _stockBalanceRepoMock.Verify(x => x.AddAsync(It.IsAny<StockBalance>()), Times.Once);
        _stockBalanceRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ProcessUploadAsync_WithExistingRecord_UpdatesRecord()
    {
        // Arrange
        var stream = new MemoryStream();
        var fileName = "stockbalance.csv";
        var csvRecords = new List<StockBalanceCsvRecord>
        {
            new StockBalanceCsvRecord
            {
                Pcode = "TT01",
                AcDate = "2024/1/1",
                Isin = "TW0001234567",
                Shares = 2000
            }
        };

        var contract = new Contract
        {
            Id = "TT1",
            ContractSeq = "001"
        };

        var equity = new Equity
        {
            StkCd = "1234",
            DealCtr = "TW"
        };

        var existingBalance = new StockBalance
        {
            Id = "TT1",
            ContractSeq = "001",
            AcDate = DateOnly.FromDateTime(new DateTime(2024, 1, 1)),
            StockNo = "1234",
            Shares = 1000
        };

        _fileParserMock.Setup(x => x.ParseAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(csvRecords);
        _contractRepoMock.Setup(x => x.FindByPcodeAsync("TT1"))
            .ReturnsAsync(contract);
        _equityRepoMock.Setup(x => x.FindByIsinAsync("TW0001234567"))
            .ReturnsAsync(equity);
        _stockBalanceRepoMock.Setup(x => x.FindByKeyAsync("TT1", "001", It.IsAny<DateOnly>(), "1234"))
            .ReturnsAsync(existingBalance);
        _stockBalanceRepoMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.ProcessUploadAsync(stream, fileName);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(0, result.RowsAdded);
        Assert.Equal(1, result.RowsUpdated);
        Assert.Equal(0, result.RowsFailed);
        Assert.Equal(2000, existingBalance.Shares);
        _stockBalanceRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ProcessUploadAsync_WithMissingContract_ReturnsFailure()
    {
        // Arrange
        var stream = new MemoryStream();
        var fileName = "stockbalance.csv";
        var csvRecords = new List<StockBalanceCsvRecord>
        {
            new StockBalanceCsvRecord
            {
                Pcode = "TT01",
                AcDate = "2024/1/1",
                Isin = "TW0001234567",
                Shares = 1000
            }
        };

        _fileParserMock.Setup(x => x.ParseAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(csvRecords);
        _contractRepoMock.Setup(x => x.FindByPcodeAsync("TT1"))
            .ReturnsAsync((Contract?)null);
        _stockBalanceRepoMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.ProcessUploadAsync(stream, fileName);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(0, result.RowsAdded);
        Assert.Equal(0, result.RowsUpdated);
        Assert.Equal(1, result.RowsFailed);
        Assert.Contains("找不到對應的 Contract", result.Message);
    }

    [Fact]
    public async Task ProcessUploadAsync_WithMissingEquity_ReturnsFailure()
    {
        // Arrange
        var stream = new MemoryStream();
        var fileName = "stockbalance.csv";
        var csvRecords = new List<StockBalanceCsvRecord>
        {
            new StockBalanceCsvRecord
            {
                Pcode = "TT01",
                AcDate = "2024/1/1",
                Isin = "TW0001234567",
                Shares = 1000
            }
        };

        var contract = new Contract
        {
            Id = "TT1",
            ContractSeq = "001"
        };

        _fileParserMock.Setup(x => x.ParseAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(csvRecords);
        _contractRepoMock.Setup(x => x.FindByPcodeAsync("TT1"))
            .ReturnsAsync(contract);
        _equityRepoMock.Setup(x => x.FindByIsinAsync("TW0001234567"))
            .ReturnsAsync((Equity?)null);
        _stockBalanceRepoMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.ProcessUploadAsync(stream, fileName);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(0, result.RowsAdded);
        Assert.Equal(0, result.RowsUpdated);
        Assert.Equal(1, result.RowsFailed);
        Assert.Contains("找不到對應的 Equity", result.Message);
    }

    [Fact]
    public async Task ProcessUploadAsync_WithInvalidDate_ReturnsFailure()
    {
        // Arrange
        var stream = new MemoryStream();
        var fileName = "stockbalance.csv";
        var csvRecords = new List<StockBalanceCsvRecord>
        {
            new StockBalanceCsvRecord
            {
                Pcode = "TT01",
                AcDate = "invalid-date",
                Isin = "TW0001234567",
                Shares = 1000
            }
        };

        var contract = new Contract
        {
            Id = "TT1",
            ContractSeq = "001"
        };

        var equity = new Equity
        {
            StkCd = "1234",
            DealCtr = "TW"
        };

        _fileParserMock.Setup(x => x.ParseAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(csvRecords);
        _contractRepoMock.Setup(x => x.FindByPcodeAsync("TT1"))
            .ReturnsAsync(contract);
        _equityRepoMock.Setup(x => x.FindByIsinAsync("TW0001234567"))
            .ReturnsAsync(equity);
        _stockBalanceRepoMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.ProcessUploadAsync(stream, fileName);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(0, result.RowsAdded);
        Assert.Equal(0, result.RowsUpdated);
        Assert.Equal(1, result.RowsFailed);
        Assert.Contains("日期格式錯誤", result.Message);
    }

    [Fact]
    public async Task ProcessUploadAsync_WithEmptyFile_ReturnsFailure()
    {
        // Arrange
        var stream = new MemoryStream();
        var fileName = "empty.csv";
        var csvRecords = new List<StockBalanceCsvRecord>();

        _fileParserMock.Setup(x => x.ParseAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(csvRecords);

        // Act
        var result = await _service.ProcessUploadAsync(stream, fileName);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(0, result.RowsAdded);
        Assert.Equal(0, result.RowsUpdated);
        Assert.Equal(0, result.RowsFailed);
        Assert.Contains("CSV 檔案為空", result.Message);
    }

    [Fact]
    public async Task ProcessUploadAsync_WithParserException_ReturnsFailure()
    {
        // Arrange
        var stream = new MemoryStream();
        var fileName = "invalid.csv";

        _fileParserMock.Setup(x => x.ParseAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("解析錯誤"));

        // Act
        var result = await _service.ProcessUploadAsync(stream, fileName);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(0, result.RowsAdded);
        Assert.Equal(0, result.RowsUpdated);
        Assert.Equal(1, result.RowsFailed);
        Assert.Contains("讀取 CSV 檔案時發生錯誤", result.Message);
    }

    [Fact]
    public async Task ProcessUploadAsync_WithMissingRequiredFields_ReturnsFailure()
    {
        // Arrange
        var stream = new MemoryStream();
        var fileName = "stockbalance.csv";
        var csvRecords = new List<StockBalanceCsvRecord>
        {
            new StockBalanceCsvRecord
            {
                Pcode = null, // 缺少必要欄位
                AcDate = "2024/1/1",
                Isin = "TW0001234567",
                Shares = 1000
            }
        };

        _fileParserMock.Setup(x => x.ParseAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(csvRecords);
        _stockBalanceRepoMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.ProcessUploadAsync(stream, fileName);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(0, result.RowsAdded);
        Assert.Equal(0, result.RowsUpdated);
        Assert.Equal(1, result.RowsFailed);
        Assert.Contains("欄位為空", result.Message);
    }
}

