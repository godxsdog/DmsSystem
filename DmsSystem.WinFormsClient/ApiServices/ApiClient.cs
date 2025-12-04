using System;
using System.Collections.Generic; // 為了 List<>
using System.IO;
using System.Net.Http;
// using System.Net.Http.Json; // 您的程式碼未使用，先註解
using System.Text.Json;     // 為了 JsonSerializer
using System.Threading;     // 為了 Timeout
using System.Threading.Tasks;
// using DmsSystem.Domain.Entities; // ApiClient 目前不需要

namespace DmsSystem.WinFormsClient.ApiServices
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        // 建構函式接收 API 基礎網址
        public ApiClient(string apiBaseUrl)
        {
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                throw new ArgumentNullException(nameof(apiBaseUrl), "API 基礎網址不能為空。");
            }

            // --- 提取自您的 InitializeHttpClient ---
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
            _httpClient.Timeout = Timeout.InfiniteTimeSpan; // 或設定固定超時
            // --- HttpClient 初始化完成 ---
        }

        // --- 【提取自您的 UploadFileAsync】通用的檔案上傳方法 ---
        // --- 注意：移除了 UI 相關的 ShowDialog 和 TextBox 操作 ---
        public async Task<(bool Success, string Message)> UploadFileAsync(string apiEndpoint, string filePath)
        {
            // 檔案存在性檢查移至 Form1
            try
            {
                using (var content = new MultipartFormDataContent())
                using (var fileStream = File.OpenRead(filePath)) // 使用傳入的 filePath
                using (var streamContent = new StreamContent(fileStream))
                {
                    content.Add(streamContent, "file", Path.GetFileName(filePath));

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
            catch (HttpRequestException ex) { return (false, $"API 連接錯誤: {ex.Message}"); }
            catch (Exception ex) { return (false, $"上傳過程中發生錯誤: {ex.Message}"); }
        }

        /// <summary>
        /// 用於反序列化 API 回應 JSON 的輔助內部類別。
        /// 【提取自您的 UploadResult】
        /// </summary>
        private class UploadResult
        {
            public string? Message { get; set; }
            public int RowsAdded { get; set; }
        }
    }
}