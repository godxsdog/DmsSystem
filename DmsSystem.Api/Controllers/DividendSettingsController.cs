using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DmsSystem.Api.Controllers;

[ApiController]
[Route("api/dividend-settings")]
public class DividendSettingsController : ControllerBase
{
    private readonly IFundDivSetService _setService;
    private readonly IFundDivObjService _objService;

    public DividendSettingsController(
        IFundDivSetService setService,
        IFundDivObjService objService)
    {
        _setService = setService;
        _objService = objService;
    }

    /// <summary>
    /// 取得配息參數設定（Step 1）
    /// </summary>
    [HttpGet("{fundNo}/{divType}")]
    public async Task<ActionResult<FundDivSetDto>> GetSet(string fundNo, string divType)
    {
        var result = await _setService.GetAsync(fundNo, divType);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// 新增/更新配息參數設定（Step 1）
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> SaveSet([FromBody] FundDivSetDto dto)
    {
        await _setService.SaveAsync(dto);
        return Ok(new { success = true, message = "配息參數設定已儲存" });
    }

    /// <summary>
    /// 取得目標配息率設定（回溯最近一筆）
    /// </summary>
    [HttpGet("targets/{fundNo}/{divType}/{effectiveDate}")]
    public async Task<ActionResult<FundDivObjDto>> GetTarget(string fundNo, string divType, string effectiveDate)
    {
        if (!DateTime.TryParse(effectiveDate, out var date))
        {
            return BadRequest("effectiveDate 格式錯誤，請使用 yyyy-MM-dd");
        }

        var result = await _objService.GetLatestAsync(fundNo, divType, date);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// 新增/更新目標配息率設定（Step 2）
    /// </summary>
    [HttpPost("targets")]
    public async Task<ActionResult> SaveTarget([FromBody] FundDivObjDto dto)
    {
        await _objService.SaveAsync(dto);
        return Ok(new { success = true, message = "目標配息率設定已儲存" });
    }
}
