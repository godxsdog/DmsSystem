using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;

namespace DmsSystem.Application.Services;

/// <summary>
/// 配息參數設定服務
/// </summary>
public class FundDivSetService : IFundDivSetService
{
    private readonly IFundDivSetRepository _repository;

    public FundDivSetService(IFundDivSetRepository repository)
    {
        _repository = repository;
    }

    public async Task<FundDivSetDto?> GetAsync(string fundNo, string divType)
    {
        var entity = await _repository.GetByFundNoAndDivTypeAsync(fundNo, divType);
        if (entity == null) return null;

        return new FundDivSetDto(
            entity.FundNo,
            entity.DivType,
            entity.Item01Seq,
            entity.Item02Seq,
            entity.Item03Seq,
            entity.Item04Seq,
            entity.Item05Seq,
            entity.Item06Seq,
            entity.Item07Seq,
            entity.Item08Seq,
            entity.Item09Seq,
            entity.Item10Seq,
            entity.CapitalType,
            entity.EmailList
        );
    }

    public async Task SaveAsync(FundDivSetDto dto)
    {
        var entity = await _repository.GetByFundNoAndDivTypeAsync(dto.FundNo, dto.DivType);
        if (entity == null)
        {
            entity = new FundDivSet
            {
                FundNo = dto.FundNo,
                DivType = dto.DivType
            };
            await _repository.AddAsync(entity);
        }

        entity.Item01Seq = dto.Item01Seq;
        entity.Item02Seq = dto.Item02Seq;
        entity.Item03Seq = dto.Item03Seq;
        entity.Item04Seq = dto.Item04Seq;
        entity.Item05Seq = dto.Item05Seq;
        entity.Item06Seq = dto.Item06Seq;
        entity.Item07Seq = dto.Item07Seq;
        entity.Item08Seq = dto.Item08Seq;
        entity.Item09Seq = dto.Item09Seq;
        entity.Item10Seq = dto.Item10Seq;
        entity.CapitalType = dto.CapitalType;
        entity.EmailList = dto.EmailList;

        await _repository.SaveChangesAsync();
    }
}
