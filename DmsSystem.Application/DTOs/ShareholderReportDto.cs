using System;

namespace DmsSystem.Application.DTOs
{
    /// <summary>
    /// 用於傳輸股東會報表查詢結果的資料傳輸物件。
    /// </summary>
    public class ShareholderReportDto
    {
        public string? StkCd { get; set; }          // 股票代號
        public string? EquityName { get; set; }     // 股票名稱
        public DateTime? ShmtDate { get; set; }     // 股東會日期
        public string? ShmtTime { get; set; }       // 時間
        public string? ChfChgYn { get; set; }       // 董監改選
        public string? AttendType { get; set; }     // 出席
        public string? OuterType { get; set; }      // 外部出席
        public string? ContractName { get; set; }   // 契約名稱
        public string? ShmtAddr { get; set; }       // 開會地點
        public decimal? Shares { get; set; }        // 持股數
        public string? TrusteeName { get; set; }    // 保管機構
        public DateTime? SsrgDate { get; set; }     // 停止過戶日
    }
}