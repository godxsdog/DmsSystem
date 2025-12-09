import { useState } from 'react';
import './Dividend.css';

interface DividendImportResult {
  success: boolean;
  inserted: number;
  updated: number;
  failed: number;
  errors: string[];
}

interface DividendConfirmResult {
  success: boolean;
  message: string;
  nav: number | null;
  unit: number | null;
  divTot: number | null;
  divRate: number | null;
  divRateMonthly: number | null;
  capitalRate: number | null;
}

export function Dividend() {
  const [file, setFile] = useState<File | null>(null);
  const [uploading, setUploading] = useState(false);
  const [importResult, setImportResult] = useState<DividendImportResult | null>(null);

  // 配息計算表單
  const [fundNo, setFundNo] = useState('');
  const [dividendDate, setDividendDate] = useState('');
  const [dividendType, setDividendType] = useState('M');
  const [calculating, setCalculating] = useState(false);
  const [confirmResult, setConfirmResult] = useState<DividendConfirmResult | null>(null);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setFile(e.target.files[0]);
      setImportResult(null);
    }
  };

  const handleImport = async () => {
    if (!file) {
      alert('請選擇檔案');
      return;
    }

    setUploading(true);
    setImportResult(null);

    try {
      const formData = new FormData();
      formData.append('file', file);

      const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5137';
      const apiUrl = `${API_BASE_URL}/api/Dividends/import`;
      
      console.log('嘗試連接 API:', apiUrl);
      
      const response = await fetch(apiUrl, {
        method: 'POST',
        body: formData,
        // 不設定 headers，讓瀏覽器自動設定 Content-Type 和 boundary
      });

      let result: DividendImportResult;
      
      try {
        const data = await response.json();
        
        // 檢查是否為後端返回的錯誤格式（GlobalExceptionHandlerMiddleware）
        if (data.error) {
          const errorMsg = data.error.message || data.error.type || '未知錯誤';
          const errorDetails = data.error.stackTrace ? `\n詳細資訊: ${data.error.stackTrace.split('\n')[0]}` : '';
          result = {
            success: false,
            inserted: 0,
            updated: 0,
            failed: 1,
            errors: [`${errorMsg}${errorDetails}`],
          };
        }
        // 檢查是否為 DividendImportResult 格式
        else if (data.success !== undefined || data.inserted !== undefined) {
          result = data as DividendImportResult;
        }
        // 其他錯誤格式
        else {
          result = {
            success: false,
            inserted: 0,
            updated: 0,
            failed: 1,
            errors: [data.message || `HTTP ${response.status}: ${response.statusText}`],
          };
        }
      } catch (parseError) {
        // JSON 解析失敗，可能是網路錯誤或其他問題
        result = {
          success: false,
          inserted: 0,
          updated: 0,
          failed: 1,
          errors: [`無法解析伺服器回應: ${response.status} ${response.statusText}`],
        };
      }

      setImportResult(result);
    } catch (error) {
      // 網路錯誤或其他例外
      let errorMessage = '匯入失敗：未知錯誤';
      
      if (error instanceof TypeError && error.message.includes('Failed to fetch')) {
        errorMessage = `無法連接到後端 API (${apiUrl})。請檢查：\n1. 後端服務是否正在運行\n2. API URL 是否正確\n3. 是否有 CORS 問題\n4. 網路連線是否正常\n\n錯誤詳情: ${error.message}`;
      } else if (error instanceof Error) {
        errorMessage = `${error.message}${error.stack ? `\n堆疊: ${error.stack.split('\n').slice(0, 3).join('\n')}` : ''}`;
      }
      
      console.error('匯入錯誤:', error);
      setImportResult({
        success: false,
        inserted: 0,
        updated: 0,
        failed: 1,
        errors: [errorMessage],
      });
    } finally {
      setUploading(false);
    }
  };

  const handleConfirm = async () => {
    if (!fundNo || !dividendDate) {
      alert('請填寫基金代號和配息基準日');
      return;
    }

    setCalculating(true);
    setConfirmResult(null);

    try {
      const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5137';
      const response = await fetch(
        `${API_BASE_URL}/api/Dividends/${fundNo}/${dividendDate}/${dividendType}/confirm`,
        {
          method: 'POST',
        }
      );

      if (!response.ok) {
        const error = await response.json().catch(() => ({ message: response.statusText }));
        throw new Error(error.message || `HTTP error! status: ${response.status}`);
      }

      const result: DividendConfirmResult = await response.json();
      setConfirmResult(result);
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : '計算失敗';
      setConfirmResult({
        success: false,
        message: errorMessage,
        nav: null,
        unit: null,
        divTot: null,
        divRate: null,
        divRateMonthly: null,
        capitalRate: null,
      });
    } finally {
      setCalculating(false);
    }
  };

  return (
    <div className="dividend-container">
      <h2>配息管理</h2>

      {/* 檔案匯入區塊 */}
      <section className="dividend-section">
        <h3>1. 匯入可分配收益 CSV 檔案</h3>
        <div className="form-group">
          <label>選擇 CSV 檔案（Big5 編碼）：</label>
          <input
            type="file"
            accept=".csv"
            onChange={handleFileChange}
            disabled={uploading}
          />
          <button onClick={handleImport} disabled={!file || uploading}>
            {uploading ? '匯入中...' : '匯入檔案'}
          </button>
        </div>

        {importResult && (
          <div className={`result-box ${importResult.success ? 'success' : 'error'}`}>
            <h4>匯入結果</h4>
            <p>新增：{importResult.inserted} 筆</p>
            <p>更新：{importResult.updated} 筆</p>
            <p>失敗：{importResult.failed} 筆</p>
            {importResult.errors.length > 0 && (
              <div className="errors">
                <strong>錯誤訊息：</strong>
                <ul>
                  {importResult.errors.map((error, index) => (
                    <li key={index}>{error}</li>
                  ))}
                </ul>
              </div>
            )}
          </div>
        )}
      </section>

      {/* 配息計算區塊 */}
      <section className="dividend-section">
        <h3>2. 執行配息計算與確認</h3>
        <div className="form-group">
          <label>
            基金代號：
            <input
              type="text"
              value={fundNo}
              onChange={(e) => setFundNo(e.target.value)}
              placeholder="例如：A001"
              disabled={calculating}
            />
          </label>
        </div>
        <div className="form-group">
          <label>
            配息基準日：
            <input
              type="date"
              value={dividendDate}
              onChange={(e) => setDividendDate(e.target.value)}
              disabled={calculating}
            />
          </label>
        </div>
        <div className="form-group">
          <label>
            配息頻率：
            <select
              value={dividendType}
              onChange={(e) => setDividendType(e.target.value)}
              disabled={calculating}
            >
              <option value="M">月配 (M)</option>
              <option value="Q">季配 (Q)</option>
              <option value="S">半年配 (S)</option>
              <option value="Y">年配 (Y)</option>
            </select>
          </label>
        </div>
        <button onClick={handleConfirm} disabled={calculating || !fundNo || !dividendDate}>
          {calculating ? '計算中...' : '執行計算與確認'}
        </button>

        {confirmResult && (
          <div className={`result-box ${confirmResult.success ? 'success' : 'error'}`}>
            <h4>計算結果</h4>
            <p className="result-message">{confirmResult.message}</p>
            {confirmResult.success && (
              <div className="result-details">
                <div className="result-row">
                  <span className="label">基準日淨值 (NAV)：</span>
                  <span className="value">{confirmResult.nav?.toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 8 }) || 'N/A'}</span>
                </div>
                <div className="result-row">
                  <span className="label">基準日單位數：</span>
                  <span className="value">{confirmResult.unit?.toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) || 'N/A'}</span>
                </div>
                <div className="result-row">
                  <span className="label">總可分配金額：</span>
                  <span className="value">{confirmResult.divTot?.toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) || 'N/A'}</span>
                </div>
                <div className="result-row">
                  <span className="label">每單位可分配收益（年化）：</span>
                  <span className="value">{confirmResult.divRate?.toLocaleString('zh-TW', { minimumFractionDigits: 6, maximumFractionDigits: 6 }) || 'N/A'}</span>
                </div>
                <div className="result-row">
                  <span className="label">每單位配息金額（當期）：</span>
                  <span className="value">{confirmResult.divRateMonthly?.toLocaleString('zh-TW', { minimumFractionDigits: 6, maximumFractionDigits: 6 }) || 'N/A'}</span>
                </div>
                <div className="result-row">
                  <span className="label">每單位本金配息比率：</span>
                  <span className="value">{confirmResult.capitalRate?.toLocaleString('zh-TW', { minimumFractionDigits: 6, maximumFractionDigits: 6 }) || 'N/A'}</span>
                </div>
              </div>
            )}
          </div>
        )}
      </section>
    </div>
  );
}
