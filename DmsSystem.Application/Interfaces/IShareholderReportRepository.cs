using DmsSystem.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DmsSystem.Application.Interfaces
{
    public interface IShareholderReportRepository
    {
        Task<IEnumerable<ShareholderReportDto>> GetReportDataAsync();
    }
}