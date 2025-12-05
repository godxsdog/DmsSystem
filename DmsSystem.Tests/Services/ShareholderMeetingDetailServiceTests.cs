using DmsSystem.Application.Interfaces;
using DmsSystem.Application.Services;
using DmsSystem.Domain.Entities;
using Moq;
using Xunit;

namespace DmsSystem.Tests.Services;

public class ShareholderMeetingDetailServiceTests
{
    private readonly Mock<IShmtSource1Repository> _repositoryMock;
    private readonly Mock<IFileParser<ShmtSource1>> _fileParserMock;
    private readonly ShareholderMeetingDetailService _service;

    public ShareholderMeetingDetailServiceTests()
    {
        _repositoryMock = new Mock<IShmtSource1Repository>();
        _fileParserMock = new Mock<IFileParser<ShmtSource1>>();
        _service = new ShareholderMeetingDetailService(_repositoryMock.Object, _fileParserMock.Object);
    }

    [Fact]
    public async Task ProcessUploadAsync_WithValidFile_ReturnsSuccess()
    {
        // Arrange
        var stream = new MemoryStream();
        var fileName = "test.xlsx";
        var entities = new List<ShmtSource1>
        {
            new ShmtSource1 { StkCd = "1234", StkName = "測試股票" }
        };

        _fileParserMock.Setup(x => x.ParseAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<List<ShmtSource1>>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessUploadAsync(stream, fileName);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, result.RowsAdded);
        _fileParserMock.Verify(x => x.ParseAsync(It.IsAny<Stream>(), fileName), Times.Once);
        _repositoryMock.Verify(x => x.AddRangeAsync(entities), Times.Once);
    }

    [Fact]
    public async Task ProcessUploadAsync_WithEmptyFile_ReturnsSuccessWithZeroRows()
    {
        // Arrange
        var stream = new MemoryStream();
        var fileName = "empty.xlsx";
        var entities = new List<ShmtSource1>();

        _fileParserMock.Setup(x => x.ParseAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(entities);

        // Act
        var result = await _service.ProcessUploadAsync(stream, fileName);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(0, result.RowsAdded);
        _repositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<ShmtSource1>>()), Times.Never);
    }

    [Fact]
    public async Task ProcessUploadAsync_WithParserException_ReturnsFailure()
    {
        // Arrange
        var stream = new MemoryStream();
        var fileName = "invalid.xlsx";

        _fileParserMock.Setup(x => x.ParseAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("解析錯誤"));

        // Act
        var result = await _service.ProcessUploadAsync(stream, fileName);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("處理檔案時發生錯誤", result.Message);
        _repositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<ShmtSource1>>()), Times.Never);
    }
}

