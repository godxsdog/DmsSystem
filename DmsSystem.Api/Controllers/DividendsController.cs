using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DmsSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DividendsController : ControllerBase
{
    private readonly IDividendService _service;
    private readonly IFundDivRepository _fundDivRepository;

    public DividendsController(IDividendService service, IFundDivRepository fundDivRepository)
    {
        _service = service;
        _fundDivRepository = fundDivRepository;
    }

    /// <summary>
    /// 5A1：匯入可分配收益 CSV 檔案
    /// </summary>
    /// <param name="file">CSV 檔案（Big5 編碼）</param>
    /// <returns>匯入結果</returns>
    [HttpPost("import")]
    public async Task<ActionResult<DividendImportResult>> Import([FromForm] IFormFile file)
    {
        var result = await _service.ImportAsync(file);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// 5A1：執行配息計算與確認
    /// </summary>
    /// <param name="fundNo">基金代號</param>
    /// <param name="dividendDate">配息基準日（格式：yyyy-MM-dd）</param>
    /// <param name="dividendType">配息頻率（M/Q/S/Y）</param>
    /// <returns>計算結果</returns>
    [HttpPost("{fundNo}/{dividendDate}/{dividendType}/confirm")]
    public async Task<ActionResult<DividendConfirmResult>> Confirm(string fundNo, string dividendDate, string dividendType)
    {
        if (!DateOnly.TryParse(dividendDate, out var dateOnly))
        {
            return BadRequest("dividendDate 格式錯誤，請使用 yyyy-MM-dd");
        }

        var result = await _service.ConfirmAsync(fundNo, dateOnly, dividendType);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// 查詢已載入的配息資料
    /// </summary>
    /// <param name="fundNo">基金代號（選填）</param>
    /// <param name="dividendType">配息頻率（選填：M/Q/S/Y）</param>
    /// <param name="startDate">開始日期（選填，格式：yyyy-MM-dd）</param>
    /// <param name="endDate">結束日期（選填，格式：yyyy-MM-dd）</param>
    /// <returns>配息資料列表</returns>
    [HttpGet]
    public async Task<ActionResult> GetDividends(
        [FromQuery] string? fundNo = null,
        [FromQuery] string? dividendType = null,
        [FromQuery] string? startDate = null,
        [FromQuery] string? endDate = null)
    {
        try
        {
            DateTime? start = null;
            DateTime? end = null;

            if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out var parsedStart))
            {
                start = parsedStart;
            }

            if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var parsedEnd))
            {
                end = parsedEnd;
            }

            var dividends = await _fundDivRepository.GetAllAsync(fundNo, dividendType, start, end);
            return Ok(new { data = dividends, total = dividends.Count() });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

