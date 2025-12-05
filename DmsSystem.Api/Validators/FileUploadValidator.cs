using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace DmsSystem.Api.Validators;

/// <summary>
/// 檔案上傳驗證器
/// </summary>
public class FileUploadValidator : AbstractValidator<IFormFile>
{
    private readonly string[] _allowedExtensions = { ".xlsx", ".csv" };
    private readonly long _maxFileSize = 10 * 1024 * 1024; // 10MB

    public FileUploadValidator()
    {
        RuleFor(file => file)
            .NotNull()
            .WithMessage("檔案不能為空");

        RuleFor(file => file.Length)
            .GreaterThan(0)
            .WithMessage("檔案大小必須大於 0")
            .LessThanOrEqualTo(_maxFileSize)
            .WithMessage($"檔案大小不能超過 {_maxFileSize / 1024 / 1024}MB");

        RuleFor(file => file.FileName)
            .Must(fileName => _allowedExtensions.Any(ext => fileName.ToLowerInvariant().EndsWith(ext)))
            .WithMessage($"檔案格式不支援，僅支援: {string.Join(", ", _allowedExtensions)}");

        RuleFor(file => file.ContentType)
            .Must(contentType => 
                contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ||
                contentType == "text/csv" ||
                contentType == "application/csv")
            .When(file => file != null)
            .WithMessage("檔案類型不正確");
    }
}

