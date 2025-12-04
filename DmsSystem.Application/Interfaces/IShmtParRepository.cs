using DmsSystem.Domain.Entities; // 引用 Domain 層的類別

namespace DmsSystem.Application.Interfaces
{
    public interface IShmtParRepository
    {
        Task<IEnumerable<ShmtPar>> GetAllAsync(); // 定義一個方法來取得所有股東會資料
    }
}