using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;

namespace DmsSystem.Application.Services;

/// <summary>
/// 目標配息率設定服務
/// </summary>
public class FundDivObjService : IFundDivObjService
{
    private readonly IFundDivObjRepository _repository;

    public FundDivObjService(IFundDivObjRepository repository)
    {
        _repository = repository;
    }

    public async Task<FundDivObjDto?> GetLatestAsync(string fundNo, string divType, DateTime effectiveDate)
    {
        var entity = await _repository.GetEffectiveAsync(fundNo, divType, effectiveDate);
        if (entity == null) return null;

        return new FundDivObjDto(
            entity.FundNo,
            entity.DivType,
            entity.TxDate,
            entity.DivObj,
            entity.DivObjAmt
        );
    }

    public async Task SaveAsync(FundDivObjDto dto)
    {
        var entity = await _repository.GetEffectiveAsync(dto.FundNo, dto.DivType, dto.TxDate);
        if (entity == null || entity.TxDate != dto.TxDate)
        {
            entity = new FundDivObj
            {
                FundNo = dto.FundNo,
                DivType = dto.DivType,
                TxDate = dto.TxDate
            };
            await _repository.AddAsync(entity);
        }
        else
        {
            await _repository.UpdateAsync(entity);
        }

        entity.DivObj = dto.DivObj;
        entity.DivObjAmt = dto.DivObjAmt;

        await _repository.SaveChangesAsync();
    }
}
