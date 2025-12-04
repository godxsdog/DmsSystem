using Dapper; // 引用 Dapper
using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using DmsSystem.Infrastructure.Persistence.Contexts; // 引用您的 DbContext
using Microsoft.EntityFrameworkCore; // 為了 .GetDbConnection()
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DmsSystem.Infrastructure.Persistence.Repositories
{
    public class ShareholderReportDapperRepository : IShareholderReportRepository
    {
        private readonly DmsDbContext _context;

        // 我們可以注入 EF Core 的 DbContext...
        public ShareholderReportDapperRepository(DmsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShareholderReportDto>> GetReportDataAsync()
        {
            // 步驟 1 翻譯的 T-SQL 語法
            var sql = @"
                SELECT
                    par.STK_CD          AS StkCd,
                    eq.NAME             AS EquityName,
                    par.SHMT_DATE       AS ShmtDate,
                    par.SHMT_TIME       AS ShmtTime,
                    par.CHF_CHG_YN      AS ChfChgYn,
                    par.ATTEND_TYPE     AS AttendType,
                    par.OUTER_TYPE      AS OuterType,
                    con.NAME            AS ContractName,
                    par.SHMT_ADDR       AS ShmtAddr,
                    holder.SHARES       AS Shares,
                    trs.NAME            AS TrusteeName,
                    par.SSRG_DATE       AS SsrgDate
                FROM
                    RIS.SHMT_PAR AS par
                LEFT JOIN
                    RIS.EQUITY AS eq ON par.STK_CD = eq.STK_CD
                LEFT JOIN
                    RIS.SHMT_PAR_HOLDER_ALL AS holder ON par.STK_CD = holder.STK_CD
                                                    AND par.SHMT_DATE = holder.SHMT_DATE
                                                    AND holder.TYPE = '1'
                LEFT JOIN
                    DMS.CONTRACT AS con ON holder.ID = con.ID
                                           AND holder.CONTRACT_SEQ = con.CONTRACT_SEQ
                CROSS JOIN -- 來自 PB 語法的隱含 CROSS JOIN，請再次確認！
                    DMS.TRUSTEE AS trs;
            ";

            // ...然後從 DbContext 取得底層的 ADO.NET 連線
            // 這樣 Dapper 就能共用 EF Core 的連線池和事務 (如果有的話)
            using (var connection = _context.Database.GetDbConnection())
            {
                // Dapper 的核心擴充方法：QueryAsync<T>
                // 它會自動執行 SQL 並將結果映射到 List<ShareholderReportDto>
                var data = await connection.QueryAsync<ShareholderReportDto>(sql);
                return data;
            }
        }
    }
}