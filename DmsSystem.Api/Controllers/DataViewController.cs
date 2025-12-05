using DmsSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DmsSystem.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DataViewController : ControllerBase
{
    private readonly IShmtSource1Repository _shmtSource1Repo;
    private readonly IShmtSource4Repository _shmtSource4Repo;
    private readonly IStockBalanceRepository _stockBalanceRepo;

    public DataViewController(
        IShmtSource1Repository shmtSource1Repo,
        IShmtSource4Repository shmtSource4Repo,
        IStockBalanceRepository stockBalanceRepo)
    {
        _shmtSource1Repo = shmtSource1Repo;
        _shmtSource4Repo = shmtSource4Repo;
        _stockBalanceRepo = stockBalanceRepo;
    }

    /// <summary>
    /// 取得股東會明細資料列表
    /// </summary>
    [HttpGet("shareholder-meetings")]
    public async Task<IActionResult> GetShareholderMeetings([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            var data = await _shmtSource1Repo.GetAllAsync();
            var total = data.Count();
            var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new
            {
                data = pagedData,
                total,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(total / (double)pageSize)
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// 取得公司資訊列表
    /// </summary>
    [HttpGet("company-info")]
    public async Task<IActionResult> GetCompanyInfo([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            var data = await _shmtSource4Repo.GetAllAsync();
            var total = data.Count();
            var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new
            {
                data = pagedData,
                total,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(total / (double)pageSize)
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// 取得股票餘額列表
    /// </summary>
    [HttpGet("stock-balance")]
    public async Task<IActionResult> GetStockBalance([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            var data = await _stockBalanceRepo.GetAllAsync();
            var total = data.Count();
            var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new
            {
                data = pagedData,
                total,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(total / (double)pageSize)
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

