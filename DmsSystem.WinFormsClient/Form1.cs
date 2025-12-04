using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DmsSystem.WinFormsClient
{
    public partial class Form1 : Form
    {
        private static HttpClient _httpClient = null!;

        public Form1(ApiServices.ApiClient apiClient)
        {
            InitializeComponent();
            InitializeHttpClient();
            WireUpEventHandlers();
        }

        // --- 以下方法 (WireUpEventHandlers, MenuItem_..._Click, SwitchPanel, InitializeHttpClient) 維持您原始版本不變 ---
        private void WireUpEventHandlers() { this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click); this.btnClose.Click += new System.EventHandler(this.btnClose_Click); this.menuItem_Shareholder_SubItem1.Click += new System.EventHandler(this.MenuItem_AReport_Click); this.menuItem_Shareholder_SubItem2.Click += new System.EventHandler(this.MenuItem_SubItem2_Click); this.menuItem_Dividend.Click += new System.EventHandler(this.MenuItem_Other_Click); this.menuItem_Mandate.Click += new System.EventHandler(this.MenuItem_Other_Click); }
        private void MenuItem_AReport_Click(object? sender, EventArgs e) { SwitchPanel(panel_A_Report); this.Text = "DMS 系統 - A報告"; }
        private void MenuItem_SubItem2_Click(object? sender, EventArgs e) { SwitchPanel(panel_SubItem2); this.Text = "DMS 系統 - 項下2"; }
        private void MenuItem_Other_Click(object? sender, EventArgs e) { SwitchPanel(null); this.Text = "DMS 系統"; ToolStripMenuItem? clickedItem = sender as ToolStripMenuItem; if (clickedItem != null) MessageBox.Show($"您點擊了 '{clickedItem.Text}'，此功能尚未實作。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void SwitchPanel(Panel? panelToShow) { if (panel_A_Report != null) panel_A_Report.Visible = false; if (panel_SubItem2 != null) panel_SubItem2.Visible = false; if (panelToShow != null) { panelToShow.Visible = true; panelToShow.Dock = DockStyle.Fill; } }
        private void InitializeHttpClient() { var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true }; _httpClient = new HttpClient(handler); _httpClient.BaseAddress = new Uri("https://localhost:7036"); _httpClient.Timeout = Timeout.InfiniteTimeSpan; }


        // --- 「確定」按鈕的點擊事件 (【核心修改】一次只處理第一個勾選項) ---
        private async void btnConfirm_Click(object? sender, EventArgs e)
        {
            if (btnConfirm == null) return ;

            btnConfirm.Enabled = false;
            this.Text = "處理中...";
            var sbResults = new StringBuilder();
            bool itemProcessed = false; // 標記是否已處理過項目

            try
            {
                if (panel_A_Report != null && panel_A_Report.Visible)
                {
                    // --- 依序檢查，找到第一個勾選的就處理並結束 ---

                    // 1. 公司基本資料 (shmtsource4)
                    if (!itemProcessed && chkShmtSource4 != null && chkShmtSource4.Checked)
                    {
                        itemProcessed = true; // 標記已處理
                        sbResults.AppendLine("開始處理公司基本資料...");
                        var result = await UploadFileAsync("/api/FileUpload/upload-shmtsource4", "請選擇公司基本資料檔案 (XLSX)", txtShmtSource4Path, 1);
                        sbResults.AppendLine($"  - 結果: {(result.Success ? "成功" : "失敗")}, 訊息: {result.Message}");
                    }

                    // 2. 股東會明細 (shmtsource1)
                    if (!itemProcessed && chkShmtSource1 != null && chkShmtSource1.Checked)
                    {
                        itemProcessed = true; // 標記已處理
                        sbResults.AppendLine("開始處理股東會明細...");
                        var result = await UploadFileAsync("/api/ShareholderMeetings/upload-shmtsource1", "請選擇股東會明細檔案 (CSV/XLSX)", txtShmtSource1Path, 2);
                        sbResults.AppendLine($"  - 結果: {(result.Success ? "成功" : "失敗")}, 訊息: {result.Message}");
                    }

                    // 3. 持股明細資料 (阿拉丁T) - StockBalance
                    if (!itemProcessed && chkHoldingDetails != null && chkHoldingDetails.Checked)
                    {
                        itemProcessed = true; // 標記已處理
                        sbResults.AppendLine("開始處理持股明細資料...");
                        var result = await UploadFileAsync("/api/StockBalance/upload", "請選擇阿拉丁持股明細檔案 (CSV)", txtHoldingDetailsPath, 2);
                        sbResults.AppendLine($"  - 結果: {(result.Success ? "成功" : "失敗")}, 訊息: {result.Message}");
                    }

                    // 如果循環完畢都沒有處理任何項目
                    if (!itemProcessed)
                    {
                        if ((chkShmtSource1 == null || !chkShmtSource1.Checked) &&
                           (chkShmtSource4 == null || !chkShmtSource4.Checked) &&
                           (chkHoldingDetails == null || !chkHoldingDetails.Checked))
                        {
                            sbResults.AppendLine("請至少勾選一個要處理的項目。");
                        }
                        else
                        {
                            // 理論上不會到這裡，除非有其他 Panel 或邏輯
                            sbResults.AppendLine("目前畫面上無可執行的確認動作。");
                        }
                    }
                }
                else { sbResults.AppendLine("目前畫面上無可執行的確認動作。"); }

                MessageBox.Show(sbResults.ToString(), "處理完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // 錯誤處理維持不變
                if (ex is TaskCanceledException && ex.InnerException is TimeoutException) { MessageBox.Show("請求超時...", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                else { MessageBox.Show($"發生未預期的錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            finally
            {
                // 恢復 UI 狀態維持不變
                if (btnConfirm != null) btnConfirm.Enabled = true;
                if (panel_A_Report != null && panel_A_Report.Visible) this.Text = "DMS 系統 - A報告";
                else if (panel_SubItem2 != null && panel_SubItem2.Visible) this.Text = "DMS 系統 - 項下2";
                else this.Text = "DMS 系統";
            }
        }

        // --- 「關閉」按鈕的點擊事件處理程式 ---
        private void btnClose_Click(object? sender, EventArgs e) { this.Close(); }

        // --- UploadFileAsync 方法 (維持您原始版本不變，包含 ShowDialog) ---
        private async Task<(bool Success, string Message)> UploadFileAsync(string apiEndpoint, string dialogTitle, TextBox pathTextBox, int defaultFilterIndex = 1)
        {
            if (pathTextBox == null) return (false, "內部錯誤：文字框未初始化。");

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // ... (ShowDialog 之前的設定維持不變) ...
                try
                {
                    string? initialDir = null;
                    if (!string.IsNullOrEmpty(pathTextBox.Text)) { try { initialDir = Path.GetDirectoryName(pathTextBox.Text); } catch { /* Ignore */ } }
                    openFileDialog.InitialDirectory = initialDir ?? "c:\\";
                }
                catch { openFileDialog.InitialDirectory = "c:\\"; }
                openFileDialog.Title = dialogTitle;
                openFileDialog.Filter = "Excel 檔案 (*.xlsx)|*.xlsx|CSV 檔案 (*.csv)|*.csv|所有檔案 (*.*)|*.*";
                openFileDialog.FilterIndex = defaultFilterIndex;
                openFileDialog.RestoreDirectory = true;

                // --- 每次呼叫都會執行 ShowDialog ---
                if (openFileDialog.ShowDialog() != DialogResult.OK)

                {
                    return (false, "使用者取消選擇檔案。");
                }
                // --- ShowDialog 結束 ---

                pathTextBox.Text = openFileDialog.FileName;
                try
                {
                    // ... (HttpClient 上傳邏輯維持不變) ...
                    using (var content = new MultipartFormDataContent())
                    using (var fileStream = File.OpenRead(openFileDialog.FileName))
                    using (var streamContent = new StreamContent(fileStream))
                    {
                        content.Add(streamContent, "file", Path.GetFileName(openFileDialog.FileName));
                        HttpResponseMessage response = await _httpClient.PostAsync(apiEndpoint, content);
                        string responseBody = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            if (string.IsNullOrEmpty(responseBody)) return (true, "上傳成功，伺服器未回傳詳細訊息。");
                            try
                            {
                                var result = JsonSerializer.Deserialize<UploadResult>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                                return (true, result?.Message ?? "上傳成功，但未收到詳細訊息。");
                            }
                            catch (JsonException) { return (true, $"上傳成功，但回應格式無法解析: {responseBody}"); }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(responseBody)) return (false, $"上傳失敗 (狀態碼: {(int)response.StatusCode} {response.ReasonPhrase})，伺服器未提供錯誤訊息。");
                            try
                            {
                                var result = JsonSerializer.Deserialize<UploadResult>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                                return (false, $"上傳失敗 ({(int)response.StatusCode}): {result?.Message ?? responseBody}");
                            }
                            catch (JsonException) { return (false, $"上傳失敗 ({(int)response.StatusCode}): {responseBody}"); }
                        }
                    }
                }
                catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException) { return (false, "上傳請求超時。"); }
                catch (Exception ex) { return (false, $"上傳過程中發生錯誤: {ex.Message}"); }
            }
        }

        // --- UploadResult 內部類別 (維持不變) ---
        private class UploadResult
        {
            public string Message { get; set; } = string.Empty;
            public int RowsAdded { get; set; }
        }

        // --- 空的 CheckBox 和多餘按鈕事件 (維持不變) ---
        private void btnConfirm_Click_1(object sender, EventArgs e) { }
        private void chkShmtSource4_CheckedChanged(object sender, EventArgs e) { }
        private void chkShmtSource1_CheckedChanged(object sender, EventArgs e) { }
        private void chkHoldingDetails_CheckedChanged(object sender, EventArgs e) { }

        private void btnClose_Click_1(object sender, EventArgs e)
        {

        }
    }
}