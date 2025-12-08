using DmsSystem.Application.DTOs;

namespace DmsSystem.Application.Interfaces;

public interface IFundDivSetService
{
    Task<FundDivSetDto?> GetAsync(string fundNo, string divType);
    Task SaveAsync(FundDivSetDto dto);
}
