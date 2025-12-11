import { useState } from 'react';
import { apiClient } from '../api/client';
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

interface BatchConfirmResult {
  totalCount: number;
  successCount: number;
  failureCount: number;
  errors: string[];
}

interface FundDiv {
  fundNo: string;
  dividendYear: number | null;
  dividendDate: string;
  dividendType: string;
  nav: number | null;
  unit: number | null;
  divTot: number | null;
  divRate: number | null;
}

export function Dividend() {
  const [activeTab, setActiveTab] = useState<'execute' | 'query'>('execute');

  const [file, setFile] = useState<File | null>(null);
  const [uploading, setUploading] = useState(false);
  const [importResult, setImportResult] = useState<DividendImportResult | null>(null);

  // 配息計算表單
  const [fundNo, setFundNo] = useState('');
  const [dividendDate, setDividendDate] = useState('');
  const [dividendType, setDividendType] = useState('M');
  const [calculating, setCalculating] = useState(false);
  const [confirmResult, setConfirmResult] = useState<DividendConfirmResult | null>(null);
  const [batchResult, setBatchResult] = useState<BatchConfirmResult | null>(null);

  // 查詢配息資料
  const [queryFundNo, setQueryFundNo] = useState('');
  const [queryDividendType, setQueryDividendType] = useState('');
  const [queryStartDate, setQueryStartDate] = useState('');
  const [queryEndDate, setQueryEndDate] = useState('');
  const [dividends, setDividends] = useState<FundDiv[]>([]);
  const [loadingDividends, setLoadingDividends] = useState(false);
  const [queryError, setQueryError] = useState<string | null>(null);

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

    const apiUrl = `${apiClient.getBaseUrl()}/api/Dividends/import`;

    try {
      const formData = new FormData();
      formData.append('file', file);

      console.log('嘗試連接 API:', apiUrl);
      
      const response = await fetch(apiUrl, {
        method: 'POST',
        body: formData,
      });

      console.log('API 回應狀態:', response.status, response.statusText);

      let result: DividendImportResult;
      
      try {
        const responseText = await response.text();
        console.log('API 回應內容:', responseText);
        
        let data: any;
        try {
          data = JSON.parse(responseText);
        } catch (parseError) {
          result = {
            success: false,
            inserted: 0,
            updated: 0,
            failed: 1,
            errors: [`伺服器回應格式錯誤: ${responseText.substring(0, 200)}`],
          };
          setImportResult(result);
          return;
        }
        
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
        } else if (data.success !== undefined || data.inserted !== undefined) {
          result = data as DividendImportResult;
        } else {
          result = {
            success: false,
            inserted: 0,
            updated: 0,
            failed: 1,
            errors: [data.message || `HTTP ${response.status}: ${response.statusText}`],
          };
        }
      } catch (parseError) {
        result = {
          success: false,
          inserted: 0,
          updated: 0,
          failed: 1,
          errors: [`無法處理伺服器回應: ${response.status} ${response.statusText}`],
        };
      }

      setImportResult(result);
    } catch (error) {
      let errorMessage = '匯入失敗：未知錯誤';
      
      if (error instanceof TypeError && error.message.includes('Failed to fetch')) {
        errorMessage = `無法連接到後端 API (${apiUrl})。請檢查連線設定。`;
      } else if (error instanceof Error) {
        errorMessage = error.message;
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
    setBatchResult(null);

    try {
      const response = await fetch(
        `${apiClient.getBaseUrl()}/api/Dividends/${fundNo}/${dividendDate}/${dividendType}/confirm`,
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

  const handleBatchConfirm = async () => {
    setCalculating(true);
    setConfirmResult(null);
    setBatchResult(null);

    try {
      let url = `${apiClient.getBaseUrl()}/api/Dividends/confirm-all`;
      if (dividendDate) {
        url += `?dividendDate=${dividendDate}`;
      }

      const response = await fetch(url, { method: 'POST' });

      if (!response.ok) {
        const error = await response.json().catch(() => ({ message: response.statusText }));
        throw new Error(error.message || `HTTP error! status: ${response.status}`);
      }

      const result: BatchConfirmResult = await response.json();
      setBatchResult(result);
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : '批量計算失敗';
      setBatchResult({
        totalCount: 0,
        successCount: 0,
        failureCount: 1,
        errors: [errorMessage]
      });
    } finally {
      setCalculating(false);
    }
  };

  const handleQueryDividends = async () => {
    setLoadingDividends(true);
    setQueryError(null);

    try {
      const params = new URLSearchParams();
      
      if (queryFundNo) params.append('fundNo', queryFundNo);
      if (queryDividendType) params.append('dividendType', queryDividendType);
      if (queryStartDate) params.append('startDate', queryStartDate);
      if (queryEndDate) params.append('endDate', queryEndDate);

      const response = await fetch(`${apiClient.getBaseUrl()}/api/Dividends?${params.toString()}`);
      
      if (!response.ok) {
        const error = await response.json().catch(() => ({ error: response.statusText }));
        throw new Error(error.error || `HTTP ${response.status}`);
      }

      const result = await response.json();
      setDividends(result.data || []);
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : '查詢失敗';
      setQueryError(errorMessage);
      setDividends([]);
    } finally {
      setLoadingDividends(false);
    }
  };

  return (
    <div className="dividend-container">
      <h2>配息管理</h2>

      <div className="tab-nav">
        <button 
          className={`tab-btn ${activeTab === 'execute' ? 'active' : ''}`}
          onClick={() => setActiveTab('execute')}
        >
          匯入與執行
        </button>
        <button 
          className={`tab-btn ${activeTab === 'query' ? 'active' : ''}`}
          onClick={() => setActiveTab('query')}
        >
          查詢
        </button>
      </div>

      {activeTab === 'execute' && (
        <>
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
                {importResult.success ? (
                  <div style={{ marginBottom: '10px', padding: '10px', backgroundColor: '#d4edda', borderRadius: '4px' }}>
                    <strong style={{ color: '#155724' }}>✓ 匯入成功！</strong>
                  </div>
                ) : (
                  <div style={{ marginBottom: '10px', padding: '10px', backgroundColor: '#f8d7da', borderRadius: '4px' }}>
                    <strong style={{ color: '#721c24' }}>✗ 匯入失敗</strong>
                  </div>
                )}
                <p><strong>新增：</strong>{importResult.inserted} 筆</p>
                <p><strong>更新：</strong>{importResult.updated} 筆</p>
                <p><strong>失敗：</strong>{importResult.failed} 筆</p>
                {importResult.errors.length > 0 && (
                  <div className="errors" style={{ marginTop: '10px' }}>
                    <strong>錯誤訊息：</strong>
                    <ul>
                      {importResult.errors.map((error, index) => (
                        <li key={index}>{error}</li>
                      ))}
                    </ul>
                  </div>
                )}
                {importResult.success && importResult.inserted === 0 && importResult.updated === 0 && (
                  <div style={{ marginTop: '10px', padding: '10px', backgroundColor: '#fff3cd', borderRadius: '4px' }}>
                    <strong style={{ color: '#856404' }}>⚠ 注意：</strong> 匯入成功但沒有新增或更新任何資料，可能是資料已存在或格式不符。
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
                  placeholder="例如：A001 (留空以使用批量計算)"
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
            <div style={{ display: 'flex', gap: '10px' }}>
              <button 
                onClick={handleConfirm} 
                disabled={calculating || !fundNo || !dividendDate} 
                style={{ flex: 1 }}
                className="btn-execute"
              >
                {calculating && fundNo ? '計算中...' : '執行單筆計算與確認'}
              </button>
              <button 
                onClick={handleBatchConfirm} 
                disabled={calculating} 
                style={{ flex: 1, backgroundColor: '#28a745', borderColor: '#28a745' }}
                title="計算所有「未確認」的項目 (可指定日期)"
                className="btn-execute"
              >
                {calculating && !fundNo ? '批量計算中...' : '批量計算所有未確認項目'}
              </button>
            </div>

            {confirmResult && (
              <div className={`result-box ${confirmResult.success ? 'success' : 'error'}`} style={{ marginTop: '20px' }}>
                <h4>單筆計算結果</h4>
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

            {batchResult && (
               <div className={`result-box ${batchResult.successCount > 0 && batchResult.failureCount === 0 ? 'success' : 'error'}`} style={{ marginTop: '20px' }}>
                <h4>批量計算結果</h4>
                <p><strong>總處理筆數：</strong>{batchResult.totalCount}</p>
                <p><strong>成功：</strong>{batchResult.successCount}</p>
                <p><strong>失敗：</strong>{batchResult.failureCount}</p>
                
                {batchResult.errors.length > 0 && (
                  <div className="errors" style={{ marginTop: '10px' }}>
                    <strong>錯誤詳情：</strong>
                    <ul>
                      {batchResult.errors.map((error, index) => (
                        <li key={index}>{error}</li>
                      ))}
                    </ul>
                  </div>
                )}
                
                {batchResult.totalCount === 0 && (
                   <div style={{ marginTop: '10px', padding: '10px', backgroundColor: '#fff3cd', borderRadius: '4px' }}>
                    <strong style={{ color: '#856404' }}>⚠ 提示：</strong> 沒有發現需要確認的項目 (STEP2_STATUS = 'C')。
                  </div>
                )}
              </div>
            )}
          </section>
        </>
      )}

      {activeTab === 'query' && (
        <section className="dividend-section">
          <h3>3. 查詢已載入的配息資料</h3>
          <div className="form-group">
            <label>
              基金代號（選填）：
              <input
                type="text"
                value={queryFundNo}
                onChange={(e) => setQueryFundNo(e.target.value)}
                placeholder="例如：D109"
              />
            </label>
          </div>
          <div className="form-group">
            <label>
              配息頻率（選填）：
              <select
                value={queryDividendType}
                onChange={(e) => setQueryDividendType(e.target.value)}
              >
                <option value="">全部</option>
                <option value="M">月配 (M)</option>
                <option value="Q">季配 (Q)</option>
                <option value="S">半年配 (S)</option>
                <option value="Y">年配 (Y)</option>
              </select>
            </label>
          </div>
          <div className="form-group">
            <label>
              開始日期（選填）：
              <input
                type="date"
                value={queryStartDate}
                onChange={(e) => setQueryStartDate(e.target.value)}
              />
            </label>
          </div>
          <div className="form-group">
            <label>
              結束日期（選填）：
              <input
                type="date"
                value={queryEndDate}
                onChange={(e) => setQueryEndDate(e.target.value)}
              />
            </label>
          </div>
          <button onClick={handleQueryDividends} disabled={loadingDividends}>
            {loadingDividends ? '查詢中...' : '查詢配息資料'}
          </button>

          {queryError && (
            <div className="result-box error">
              <h4>查詢錯誤</h4>
              <p>{queryError}</p>
            </div>
          )}

          {dividends.length > 0 && (
            <div className="result-box success">
              <h4>查詢結果（共 {dividends.length} 筆）</h4>
              <div style={{ overflowX: 'auto', marginTop: '10px' }}>
                <table className="dividend-table">
                  <thead>
                    <tr>
                      <th>基金代號</th>
                      <th>配息年度</th>
                      <th>配息基準日</th>
                      <th>配息頻率</th>
                      <th className="text-right">NAV</th>
                      <th className="text-right">單位數</th>
                      <th className="text-right">總可分配金額</th>
                      <th className="text-right">配息率</th>
                    </tr>
                  </thead>
                  <tbody>
                    {dividends.map((div, index) => (
                      <tr key={index}>
                        <td>{div.fundNo}</td>
                        <td>{div.dividendYear || '-'}</td>
                        <td>{new Date(div.dividendDate).toLocaleDateString('zh-TW')}</td>
                        <td>{div.dividendType}</td>
                        <td className="text-right">
                          {div.nav?.toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 8 }) || '-'}
                        </td>
                        <td className="text-right">
                          {div.unit?.toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) || '-'}
                        </td>
                        <td className="text-right">
                          {div.divTot?.toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) || '-'}
                        </td>
                        <td className="text-right">
                          {div.divRate?.toLocaleString('zh-TW', { minimumFractionDigits: 6, maximumFractionDigits: 6 }) || '-'}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          )}
        </section>
      )}
    </div>
  );
}
