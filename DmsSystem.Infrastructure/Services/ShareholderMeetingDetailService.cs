using CsvHelper;
using CsvHelper.Configuration;
using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DmsSystem.Infrastructure.Services
{
    /// <summary>
    /// 這是為 CsvHelper 建立的「翻譯地圖」。
    /// 【已修改】現在它將 C# 屬性對應到固定的欄位「順序 (Index)」而非「名稱 (Name)」。
    /// </summary>
    public sealed class ShmtSource1Map : ClassMap<ShmtSource1>
    {
        public ShmtSource1Map()
        {
            // .Index() 是從 0 開始計算的
            // Index(0) 代表 CSV/Excel 檔案中的第 1 欄
            Map(m => m.StkCd).Index(0);       // 第 1 欄 -> 股票代號
            Map(m => m.StkName).Index(1);     // 第 2 欄 -> 股票名稱
            Map(m => m.ShmtDate).Index(2);    // 第 3 欄 -> 股東會日期
            Map(m => m.SsrgDate).Index(3);    // 第 4 欄 -> 停止過戶起
            Map(m => m.ChfChgYn).Index(4);    // 第 5 欄 -> 董監有
            Map(m => m.ShmtAddr).Index(5);    // 第 6 欄 -> 開會地點
            Map(m => m.Type).Index(6);        // 第 7 欄 -> 臨時(常)會
        }
    }

    /// <summary>
    /// 實作處理「股東會明細」檔案上傳的服務。
    /// </summary>
    public class ShareholderMeetingDetailService : IShareholderMeetingDetailService
    {
        private readonly IShmtSource1Repository _repository;

        public ShareholderMeetingDetailService(IShmtSource1Repository repository)
        {
            _repository = repository;
        }

        public async Task<(bool Success, string Message, int RowsAdded)> ProcessUploadAsync(Stream fileStream, string fileName)
        {
            var entitiesToInsert = new List<ShmtSource1>();
            string fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

            try
            {
                if (fileExtension == ".xlsx")
                {
                    entitiesToInsert = ParseXlsxStream(fileStream);
                }
                else if (fileExtension == ".csv")
                {
                    entitiesToInsert = ParseCsvStream(fileStream);
                }
                else
                {
                    return (false, "不支援的檔案格式，請上傳 .xlsx 或 .csv 檔案。", 0);
                }

                if (entitiesToInsert.Count > 0)
                {
                    await _repository.AddRangeAsync(entitiesToInsert);
                }

                return (true, $"成功載入 {entitiesToInsert.Count} 筆資料到 ris.shmtsource1。", entitiesToInsert.Count);
            }
            catch (System.Exception ex)
            {
                return (false, $"處理檔案時發生錯誤: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// 使用 CsvHelper 解析 .csv 檔案串流（依照欄位順序）。
        /// </summary>
        private List<ShmtSource1> ParseCsvStream(Stream fileStream)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // 【修改】告知 CsvHelper 檔案沒有標頭，或我們想忽略標頭，從第一行就開始讀取資料。
                HasHeaderRecord = false,
                Encoding = Encoding.GetEncoding("Big5")
            };

            using (var reader = new StreamReader(fileStream, config.Encoding))
            using (var csv = new CsvReader(reader, config))
            {
                // 註冊我們定義好的「順序對應地圖」
                csv.Context.RegisterClassMap<ShmtSource1Map>();

                // 讀取紀錄。如果 HasHeaderRecord 為 false，它會從第一行開始讀。
                // 如果您確定檔案第一行永遠是標頭且不想讀取，可以先執行一次 csv.Read()。
                csv.Read(); // 讀取並忽略第一行（標頭）
//                csv.ReadHeader(); // 這一步是可選的，但有助於某些情境下的偵錯

                var records = csv.GetRecords<ShmtSource1>().ToList();

                // 套用固定業務邏輯
                foreach (var record in records)
                {
                    record.AcDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    record.EmpNo = "A00994";
                    record.Status = "Y";
                }
                return records;
            }
        }

        /// <summary>
        /// 使用 NPOI 解析 .xlsx 檔案串流（依照欄位順序）。
        /// </summary>
        private List<ShmtSource1> ParseXlsxStream(Stream fileStream)
        {
            var entities = new List<ShmtSource1>();
            IWorkbook workbook = new XSSFWorkbook(fileStream);
            ISheet worksheet = workbook.GetSheetAt(0);

            // 從第二行開始讀取資料 (row 1)，跳過第一行的標頭 (row 0)
            for (int row = 1; row <= worksheet.LastRowNum; row++)
            {
                IRow currentRow = worksheet.GetRow(row);
                if (currentRow == null) continue;

                var entity = new ShmtSource1
                {
                    AcDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    EmpNo = "A00994",
                    Status = "Y",

                    // 【修改】直接依照固定的欄位索引讀取，不再依賴標頭名稱
                    StkCd = GetCellStringValue(currentRow.GetCell(0)),     // 第 1 欄
                    StkName = GetCellStringValue(currentRow.GetCell(1)),   // 第 2 欄
                    ShmtDate = GetCellStringValue(currentRow.GetCell(2)),  // 第 3 欄
                    SsrgDate = GetCellStringValue(currentRow.GetCell(3)),  // 第 4 欄
                    ChfChgYn = GetCellStringValue(currentRow.GetCell(4)),  // 第 5 欄
                    ShmtAddr = GetCellStringValue(currentRow.GetCell(5)),  // 第 6 欄
                    Type = GetCellStringValue(currentRow.GetCell(6))       // 第 7 欄
                };
                entities.Add(entity);
            }
            return entities;
        }

        /// <summary>
        /// 安全地取得儲存格的字串值
        /// </summary>
        private string GetCellStringValue(ICell cell)
        {
            if (cell == null) return string.Empty;
            return cell.ToString().Trim();
        }
    }
}