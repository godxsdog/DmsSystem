using DmsSystem.Application.DTOs;

namespace DmsSystem.Application.Interfaces;

public interface IFundDivObjService
{
    Task<FundDivObjDto?> GetLatestAsync(string fundNo, string divType, DateTime effectiveDate);
    Task SaveAsync(FundDivObjDto dto);
}
