using DmsSystem.Application.Validators;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace DmsSystem.Tests.Validators;

public class FileUploadValidatorTests
{
    private readonly FileUploadValidator _validator;

    public FileUploadValidatorTests()
    {
        _validator = new FileUploadValidator();
    }

    [Fact]
    public void Validate_WithNullFile_ShouldHaveError()
    {
        // Act
        var result = _validator.TestValidate((IFormFile)null!);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Validate_WithValidXlsxFile_ShouldSucceed()
    {
        // Arrange
        var file = CreateMockFile("test.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 1024);

        // Act
        var result = _validator.TestValidate(file.Object);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidCsvFile_ShouldSucceed()
    {
        // Arrange
        var file = CreateMockFile("test.csv", "text/csv", 1024);

        // Act
        var result = _validator.TestValidate(file.Object);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithInvalidExtension_ShouldHaveError()
    {
        // Arrange
        var file = CreateMockFile("test.txt", "text/plain", 1024);

        // Act
        var result = _validator.TestValidate(file.Object);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FileName);
    }

    [Fact]
    public void Validate_WithFileTooLarge_ShouldHaveError()
    {
        // Arrange
        var file = CreateMockFile("test.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 11 * 1024 * 1024); // 11MB

        // Act
        var result = _validator.TestValidate(file.Object);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Length);
    }

    [Fact]
    public void Validate_WithEmptyFile_ShouldHaveError()
    {
        // Arrange
        var file = CreateMockFile("test.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 0);

        // Act
        var result = _validator.TestValidate(file.Object);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Length);
    }

    private Mock<IFormFile> CreateMockFile(string fileName, string contentType, long length)
    {
        var file = new Mock<IFormFile>();
        file.Setup(f => f.FileName).Returns(fileName);
        file.Setup(f => f.ContentType).Returns(contentType);
        file.Setup(f => f.Length).Returns(length);
        return file;
    }
}

