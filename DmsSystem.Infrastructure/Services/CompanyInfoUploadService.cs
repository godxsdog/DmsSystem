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

    public sealed class ShmtSource4Map : ClassMap<ShmtSource4>
    {
        public ShmtSource4Map()
        {
            // 告訴 CsvHelper，中文標頭和 C# 屬性的對應關係
            Map(m => m.StkCd).Name("股票代號");
            Map(m => m.StkName).Name("股票名稱");
            Map(m => m.CompName).Name("公司名稱");
            Map(m => m.Tel).Name("電話");
            Map(m => m.Addr).Name("地址");
            Map(m => m.BrokerName).Name("股票過戶機構");
            Map(m => m.BrokerTel).Name("股務代理電話");
            Map(m => m.Spokesman).Name("發言人");
            Map(m => m.President).Name("總經理");
            Map(m => m.Chairman).Name("董事長");
            Map(m => m.IdNo).Name("統一編號");
        }
    }

    public class CompanyInfoUploadService : ICompanyInfoUploadService
    {
        private readonly IShmtSource4Repository _repository;

        public CompanyInfoUploadService(IShmtSource4Repository repository)
        {
            _repository = repository;
        }

        public async Task<(bool Success, string Message, int RowsAdded)> ProcessShmtSource4UploadAsync(Stream fileStream, string fileName)
        {
            var entitiesToInsert = new List<ShmtSource4>();
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

                return (true, $"成功載入 {entitiesToInsert.Count} 筆資料。", entitiesToInsert.Count);
            }
            catch (Exception ex)
            {
                return (false, $"處理檔案時發生錯誤: {ex.Message}", 0);
            }
        }

        private List<ShmtSource4> ParseCsvStream(Stream fileStream)
        {
            // 1. 使用我們在 Program.cs 註冊的 "Big5" 編碼來讀取檔案
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Encoding = Encoding.GetEncoding("Big5") // 指定編碼
            };

            using (var reader = new StreamReader(fileStream, config.Encoding))
            using (var csv = new CsvReader(reader, config))
            {
                // 2. 告訴 CsvHelper 使用我們定義的「翻譯地圖」
                csv.Context.RegisterClassMap<ShmtSource4Map>();

                var records = csv.GetRecords<ShmtSource4>().ToList();

                // 3. 對讀取出來的每一筆資料，套用固定的業務邏輯
                foreach (var record in records)
                {
                    record.AcDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    record.EmpNo = "A00994";
                }
                return records;
            }
        }

        private List<ShmtSource4> ParseXlsxStream(Stream fileStream)
        {
            var entities = new List<ShmtSource4>();
            IWorkbook workbook = new XSSFWorkbook(fileStream);
            ISheet worksheet = workbook.GetSheetAt(0);
            int rowCount = worksheet.LastRowNum;
            for (int row = 1; row <= rowCount; row++)
            {
                IRow currentRow = worksheet.GetRow(row);
                if (currentRow == null) continue;
                entities.Add(CreateEntityFromRow(
                    acDate: DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), empNo: "A00994",
                    stkCd: GetCellStringValue(currentRow.GetCell(0)), stkName: GetCellStringValue(currentRow.GetCell(1)),
                    compName: GetCellStringValue(currentRow.GetCell(2)), tel: GetCellStringValue(currentRow.GetCell(3)),
                    addr: GetCellStringValue(currentRow.GetCell(4)), brokerName: GetCellStringValue(currentRow.GetCell(5)),
                    brokerTel: GetCellStringValue(currentRow.GetCell(6)), spokesman: GetCellStringValue(currentRow.GetCell(7)),
                    president: GetCellStringValue(currentRow.GetCell(8)), chairman: GetCellStringValue(currentRow.GetCell(9)),
                    idNo: GetCellStringValue(currentRow.GetCell(10))
                ));
            }
            return entities;
        }
        private ShmtSource4 CreateEntityFromRow(string acDate, string empNo, string stkCd, string stkName, string compName, string tel, string addr, string brokerName, string brokerTel, string spokesman, string president, string chairman, string idNo)
        {
            return new ShmtSource4 { AcDate = acDate, EmpNo = empNo, StkCd = stkCd, StkName = stkName, CompName = compName, Tel = tel, Addr = addr, BrokerName = brokerName, BrokerTel = brokerTel, Spokesman = spokesman, President = president, Chairman = chairman, IdNo = idNo };
        }
        private string GetCellStringValue(ICell cell)
        {
            if (cell == null) return string.Empty;
            return cell.ToString().Trim();
        }
    }
}