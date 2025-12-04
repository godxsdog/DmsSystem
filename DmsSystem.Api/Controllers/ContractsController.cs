using DmsSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DmsSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly IContractRepository _contractRepository;

        public ContractsController(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var contracts = await _contractRepository.GetAllAsync();
            return Ok(contracts);
        }
    }
}