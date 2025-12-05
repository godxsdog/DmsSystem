using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using DmsSystem.Infrastructure.FileParsing;
using System.Text;
using Xunit;

namespace DmsSystem.Tests.FileParsing;

public class FileParserTests
{
    [Fact]
    public async Task ShmtSource1FileParser_ParseCsv_ShouldParseCorrectly()
    {
        // Arrange
        var csvContent = "股票代號,股票名稱,股東會日期,停止過戶起,董監有,開會地點,臨時(常)會\n1234,測試股票,2024/1/1,2023/12/1,Y,台北市,常會";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var parser = new ShmtSource1FileParser();

        // Act
        var result = await parser.ParseAsync(stream, "test.csv");

        // Assert
        Assert.Single(result);
        Assert.Equal("1234", result[0].StkCd);
        Assert.Equal("測試股票", result[0].StkName);
    }

    [Fact]
    public async Task ShmtSource4FileParser_ParseCsv_ShouldParseCorrectly()
    {
        // Arrange
        var csvContent = "股票代號,股票名稱,公司名稱,電話,地址,股票過戶機構,股務代理電話,發言人,總經理,董事長,統一編號\n1234,測試股票,測試公司,02-12345678,台北市,過戶機構,02-87654321,發言人,總經理,董事長,12345678";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var parser = new ShmtSource4FileParser();

        // Act
        var result = await parser.ParseAsync(stream, "test.csv");

        // Assert
        Assert.Single(result);
        Assert.Equal("1234", result[0].StkCd);
        Assert.Equal("測試公司", result[0].CompName);
    }

    [Fact]
    public async Task StockBalanceCsvParser_ParseCsv_ShouldParseCorrectly()
    {
        // Arrange
        var csvContent = "PCODE,AC_DATE,ISIN,SHARES\nTT01,2024/1/1,TW0001234567,1000";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
        var parser = new StockBalanceCsvParser();

        // Act
        var result = await parser.ParseAsync(stream, "test.csv");

        // Assert
        Assert.Single(result);
        Assert.Equal("TT01", result[0].Pcode);
        Assert.Equal("2024/1/1", result[0].AcDate);
        Assert.Equal("TW0001234567", result[0].Isin);
        Assert.Equal(1000, result[0].Shares);
    }

    [Fact]
    public async Task ShmtSource1FileParser_ParseEmptyFile_ShouldReturnEmptyList()
    {
        // Arrange
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(""));
        var parser = new ShmtSource1FileParser();

        // Act
        var result = await parser.ParseAsync(stream, "empty.csv");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task ShmtSource1FileParser_ParseInvalidCsv_ShouldThrowException()
    {
        // Arrange
        var invalidContent = "這不是有效的 CSV 格式";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(invalidContent));
        var parser = new ShmtSource1FileParser();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await parser.ParseAsync(stream, "invalid.csv"));
    }
}

