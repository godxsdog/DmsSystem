using DmsSystem.Application.Interfaces;
using DmsSystem.Application.Services;
using DmsSystem.Domain.Entities;
using Moq;
using Xunit;

namespace DmsSystem.Tests.Services;

public class CompanyInfoUploadServiceTests
{
    private readonly Mock<IShmtSource4Repository> _repositoryMock;
    private readonly Mock<IFileParser<ShmtSource4>> _fileParserMock;
    private readonly CompanyInfoUploadService _service;

    public CompanyInfoUploadServiceTests()
    {
        _repositoryMock = new Mock<IShmtSource4Repository>();
        _fileParserMock = new Mock<IFileParser<ShmtSource4>>();
        _service = new CompanyInfoUploadService(_repositoryMock.Object, _fileParserMock.Object);
    }

    [Fact]
    public async Task ProcessShmtSource4UploadAsync_WithValidFile_ReturnsSuccess()
    {
        // Arrange
        var stream = new MemoryStream();
        var fileName = "company.xlsx";
        var entities = new List<ShmtSource4>
        {
            new ShmtSource4 { StkCd = "1234", CompName = "測試公司" }
        };

        _fileParserMock.Setup(x => x.ParseAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(entities);
        _repositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<List<ShmtSource4>>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessShmtSource4UploadAsync(stream, fileName);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, result.RowsAdded);
    }
}

