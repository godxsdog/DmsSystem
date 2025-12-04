using Microsoft.Extensions.Configuration;
using DmsSystem.WinFormsClient.ApiServices;
using System;
using System.IO;
using System.Windows.Forms;

namespace DmsSystem.WinFormsClient
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            IConfiguration configuration;
            try
            {
                configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
            }
            catch (Exception ex) { MessageBox.Show($"讀取設定檔錯誤: {ex.Message}", "啟動失敗"); return; }

            string? apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl");
            if (string.IsNullOrEmpty(apiBaseUrl)) { MessageBox.Show("錯誤：找不到 ApiBaseUrl 設定...", "啟動失敗"); return; }

            ApiClient apiClient;
            try { apiClient = new ApiClient(apiBaseUrl); }
            catch (Exception ex) { MessageBox.Show($"建立 API Client 錯誤: {ex.Message}", "啟動失敗"); return; }

            ApplicationConfiguration.Initialize();
            // 只傳入 ApiClient
            Application.Run(new Form1(apiClient));
        }
    }
}