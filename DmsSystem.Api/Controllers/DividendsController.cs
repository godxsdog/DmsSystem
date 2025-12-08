using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DmsSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DividendsController : ControllerBase
{
    private readonly IDividendService _service;

    public DividendsController(IDividendService service)
    {
        _service = service;
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
}

