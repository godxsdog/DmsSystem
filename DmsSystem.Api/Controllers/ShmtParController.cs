using DmsSystem.Application.Interfaces; // 引用介面
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DmsSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShmtParController : ControllerBase
    {
        private readonly IShmtParRepository _shmtParRepository;

        // 透過建構函式注入 IShmtParRepository
        public ShmtParController(IShmtParRepository shmtParRepository)
        {
            _shmtParRepository = shmtParRepository;
        }

        [HttpGet] // 定義這是一個 HTTP GET 請求
        public async Task<IActionResult> GetAll()
        {
            var meetings = await _shmtParRepository.GetAllAsync();
            return Ok(meetings); // 回傳 200 OK 與查詢到的資料
        }
    }
}