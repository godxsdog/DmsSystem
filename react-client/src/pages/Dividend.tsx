import { useState, useRef, useMemo } from 'react';
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
  // 5A3 新增欄位
  interestRate?: number;
  capitalRate?: number;
  status?: string; // 單位配息狀態
  statusC?: string; // 配息組成狀態
}

type SortConfig = {
  key: keyof FundDiv | null;
  direction: 'asc' | 'desc';
};

const useSortableData = (items: FundDiv[], config: SortConfig = { key: null, direction: 'asc' }) => {
  const [sortConfig, setSortConfig] = useState<SortConfig>(config);

  const sortedItems = useMemo(() => {
    let sortableItems = [...items];
    if (sortConfig.key !== null) {
      sortableItems.sort((a, b) => {
        let aValue: any = a[sortConfig.key!];
        let bValue: any = b[sortConfig.key!];

        // Handle null/undefined
        if (aValue === null || aValue === undefined) aValue = '';
        if (bValue === null || bValue === undefined) bValue = '';

        if (aValue < bValue) {
          return sortConfig.direction === 'asc' ? -1 : 1;
        }
        if (aValue > bValue) {
          return sortConfig.direction === 'asc' ? 1 : -1;
        }
        return 0;
      });
    }
    return sortableItems;
  }, [items, sortConfig]);

  const requestSort = (key: keyof FundDiv) => {
    let direction: 'asc' | 'desc' = 'asc';
    if (sortConfig.key === key && sortConfig.direction === 'asc') {
      direction = 'desc';
    }
    setSortConfig({ key, direction });
  };

  return { items: sortedItems, requestSort, sortConfig };
};

export function Dividend() {
  const [activeTab, setActiveTab] = useState<'execute' | 'query' | 'composition'>('execute');

  const [file, setFile] = useState<File | null>(null);
  const [uploading, setUploading] = useState(false);
  const [importResult, setImportResult] = useState<DividendImportResult | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  // 配息計算表單
  const [fundNo, setFundNo] = useState('');
  const [dividendDate, setDividendDate] = useState('');
  const [dividendType, setDividendType] = useState('M');
  const [calculating, setCalculating] = useState(false);
  const [confirmResult, setConfirmResult] = useState<DividendConfirmResult | null>(null);
  const [batchResult, setBatchResult] = useState<BatchConfirmResult | null>(null);

  // 查詢配息資料 (獨立狀態)
  const [queryFundNo, setQueryFundNo] = useState('');
  const [queryDividendType, setQueryDividendType] = useState('');
  const [queryStartDate, setQueryStartDate] = useState('');
  const [queryEndDate, setQueryEndDate] = useState('');
  const [queryDividends, setQueryDividends] = useState<FundDiv[]>([]);
  const [queryLoading, setQueryLoading] = useState(false);
  const [queryError, setQueryError] = useState<string | null>(null);

  // 5A3 配息組成與上傳 EC (獨立狀態)
  const [compositionFile, setCompositionFile] = useState<File | null>(null);
  const [compositionImportResult, setCompositionImportResult] = useState<DividendImportResult | null>(null);
  const compositionFileInputRef = useRef<HTMLInputElement>(null);
  const [compFundNo, setCompFundNo] = useState('');
  const [compStartDate, setCompStartDate] = useState('');
  const [compDividends, setCompDividends] = useState<FundDiv[]>([]);
  const [compLoading, setCompLoading] = useState(false);
  const [compError, setCompError] = useState<string | null>(null);
  
  const [selectedDividends, setSelectedDividends] = useState<Set<string>>(new Set());
  const [uploadingToEc, setUploadingToEc] = useState(false);
  const [ecUploadResult, setEcUploadResult] = useState<{success: number, failed: number, errors: string[]} | null>(null);

  // Sorting
  const { items: sortedQueryDividends, requestSort: requestQuerySort, sortConfig: querySortConfig } = useSortableData(queryDividends);
  const { items: sortedCompDividends, requestSort: requestCompSort, sortConfig: compSortConfig } = useSortableData(compDividends);

  const getSortClass = (key: keyof FundDiv, config: SortConfig) => {
    if (config.key === key) {
      return `sortable ${config.direction}`;
    }
    return 'sortable';
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setFile(e.target.files[0]);
      setImportResult(null);
    }
  };

  const handleCompositionFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setCompositionFile(e.target.files[0]);
      setCompositionImportResult(null);
    }
  };

  const triggerFileSelect = () => {
    fileInputRef.current?.click();
  };

  const triggerCompositionFileSelect = () => {
    compositionFileInputRef.current?.click();
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

      const response = await fetch(apiUrl, {
        method: 'POST',
        body: formData,
      });

      let result: DividendImportResult;
      
      try {
        const responseText = await response.text();
        
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

  const handleCompositionImport = async () => {
    if (!compositionFile) {
      alert('請選擇檔案');
      return;
    }

    setUploading(true);
    setCompositionImportResult(null);

    const apiUrl = `${apiClient.getBaseUrl()}/api/Dividends/composition/import`;

    try {
      const formData = new FormData();
      formData.append('file', compositionFile);

      const response = await fetch(apiUrl, { method: 'POST', body: formData });
      const data = await response.json();

      if (!response.ok) {
        throw new Error(data.error || data.message || '匯入失敗');
      }

      setCompositionImportResult(data as DividendImportResult);
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : '匯入失敗';
      setCompositionImportResult({
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

  // 一般查詢 (Tab 2)
  const handleQueryDividends = async () => {
    setQueryLoading(true);
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
      setQueryDividends(result.data || []);
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : '查詢失敗';
      setQueryError(errorMessage);
      setQueryDividends([]);
    } finally {
      setQueryLoading(false);
    }
  };

  // 組成查詢 (Tab 3)
  const handleCompQuery = async () => {
    setCompLoading(true);
    setCompError(null);
    setSelectedDividends(new Set()); // 清除選取

    try {
      const params = new URLSearchParams();
      
      if (compFundNo) params.append('fundNo', compFundNo);
      if (compStartDate) params.append('startDate', compStartDate);

      const response = await fetch(`${apiClient.getBaseUrl()}/api/Dividends?${params.toString()}`);
      
      if (!response.ok) {
        const error = await response.json().catch(() => ({ error: response.statusText }));
        throw new Error(error.error || `HTTP ${response.status}`);
      }

      const result = await response.json();
      setCompDividends(result.data || []);
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : '查詢失敗';
      setCompError(errorMessage);
      setCompDividends([]);
    } finally {
      setCompLoading(false);
    }
  };

  // 5A3: 處理勾選
  const handleCheckboxChange = (key: string, checked: boolean) => {
    const newSelected = new Set(selectedDividends);
    if (checked) {
      newSelected.add(key);
    } else {
      newSelected.delete(key);
    }
    setSelectedDividends(newSelected);
  };

  const handleRowClick = (e: React.MouseEvent<HTMLTableRowElement>, key: string) => {
    if ((e.target as HTMLElement).tagName === 'INPUT') {
      return;
    }
    const isSelected = selectedDividends.has(key);
    handleCheckboxChange(key, !isSelected);
  };

  // 5A3: 批量上傳 EC
  const handleBatchUploadEc = async () => {
    if (selectedDividends.size === 0) {
      alert('請先勾選要上傳的項目');
      return;
    }

    if (!confirm(`確定要上傳 ${selectedDividends.size} 筆資料至 EC 官網嗎？`)) {
      return;
    }

    setUploadingToEc(true);
    setEcUploadResult(null);

    let success = 0;
    let failed = 0;
    const errors: string[] = [];

    for (const key of selectedDividends) {
      const [fNo, dDate, dType] = key.split('_'); 
      const dateOnly = dDate.split('T')[0];

      try {
        const response = await fetch(
          `${apiClient.getBaseUrl()}/api/Dividends/${fNo}/${dateOnly}/${dType}/upload-ec`,
          { method: 'POST' }
        );

        if (response.ok) {
          success++;
        } else {
          failed++;
          const err = await response.json();
          errors.push(`${fNo}: ${err.error || err.message || '上傳失敗'}`);
        }
      } catch (error) {
        failed++;
        errors.push(`${fNo}: ${error instanceof Error ? error.message : '網路錯誤'}`);
      }
    }

    setEcUploadResult({ success, failed, errors });
    setUploadingToEc(false);
    
    // 重新查詢以更新狀態
    handleCompQuery();
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
        <button 
          className={`tab-btn ${activeTab === 'composition' ? 'active' : ''}`}
          onClick={() => setActiveTab('composition')}
        >
          上傳 EC 官網
        </button>
      </div>

      {activeTab === 'execute' && (
        // ... (保持不變)
        <>
          <section className="dividend-section">
            <h3>1. 匯入可分配收益 CSV 檔案</h3>
            <div className="form-group">
              <label style={{marginBottom: '10px'}}>選擇 CSV 檔案（Big5 編碼）：</label>
              
              <div className="file-input-wrapper" style={{ marginBottom: '15px' }}>
                <input
                  type="file"
                  accept=".csv"
                  onChange={handleFileChange}
                  disabled={uploading}
                  ref={fileInputRef}
                />
                <button 
                  type="button" 
                  className="btn btn-secondary" 
                  onClick={triggerFileSelect}
                  disabled={uploading}
                >
                  {uploading ? '處理中...' : '選擇檔案'}
                </button>
                <span className="file-name">
                  {file ? file.name : '未選擇任何檔案'}
                </span>
              </div>
              
              <button 
                onClick={handleImport} 
                disabled={!file || uploading}
                className="btn btn-primary"
                style={{ width: '100%', maxWidth: '400px' }}
              >
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

          <section className="dividend-section">
            <h3>2. 執行配息計算與確認</h3>
            <div className="form-group">
              <label>
                基金代號：
                <input
                  type="text"
                  className="form-control"
                  value={fundNo}
                  onChange={(e) => setFundNo(e.target.value)}
                  placeholder="例如：A001 (留空以使用批量計算)"
                  disabled={calculating}
                  maxLength={20}
                />
              </label>
            </div>
            <div className="form-group">
              <label>
                配息基準日：
                <input
                  type="date"
                  className="form-control"
                  value={dividendDate}
                  onChange={(e) => setDividendDate(e.target.value)}
                  disabled={calculating}
                  max="9999-12-31"
                />
              </label>
            </div>
            <div className="form-group">
              <label>
                配息頻率：
                <select
                  className="form-control"
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
            <div style={{ display: 'flex', gap: '10px', maxWidth: '400px' }}>
              <button 
                onClick={handleConfirm} 
                disabled={calculating || !fundNo || !dividendDate} 
                style={{ flex: 1 }}
                className="btn btn-primary btn-execute"
              >
                {calculating && fundNo ? '計算中...' : '執行單筆計算'}
              </button>
              <button 
                onClick={handleBatchConfirm} 
                disabled={calculating} 
                style={{ flex: 1 }}
                className="btn btn-success btn-execute"
                title="計算所有「未確認」的項目 (可指定日期)"
              >
                {calculating && !fundNo ? '批量計算中...' : '批量計算'}
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
                className="form-control"
                value={queryFundNo}
                onChange={(e) => setQueryFundNo(e.target.value)}
                placeholder="例如：D109"
                maxLength={20}
              />
            </label>
          </div>
          <div className="form-group">
            <label>
              配息頻率（選填）：
              <select
                className="form-control"
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
                className="form-control"
                value={queryStartDate}
                onChange={(e) => setQueryStartDate(e.target.value)}
                max="9999-12-31"
              />
            </label>
          </div>
          <div className="form-group">
            <label>
              結束日期（選填）：
              <input
                type="date"
                className="form-control"
                value={queryEndDate}
                onChange={(e) => setQueryEndDate(e.target.value)}
                max="9999-12-31"
              />
            </label>
          </div>
          <button 
            onClick={handleQueryDividends} 
            disabled={queryLoading}
            className="btn btn-primary"
            style={{ width: '100%', maxWidth: '400px' }}
          >
            {queryLoading ? '查詢中...' : '查詢配息資料'}
          </button>

          {queryError && (
            <div className="result-box error">
              <h4>查詢錯誤</h4>
              <p>{queryError}</p>
            </div>
          )}

          {sortedQueryDividends.length > 0 && (
            <div className="result-box success">
              <h4>查詢結果（共 {sortedQueryDividends.length} 筆）</h4>
              <div className="table-wrapper" style={{ marginTop: '10px' }}>
                <table className="dividend-table">
                  <thead>
                    <tr>
                      <th onClick={() => requestQuerySort('fundNo')} className={getSortClass('fundNo', querySortConfig)}>基金代號</th>
                      <th onClick={() => requestQuerySort('dividendYear')} className={getSortClass('dividendYear', querySortConfig)}>配息年度</th>
                      <th onClick={() => requestQuerySort('dividendDate')} className={getSortClass('dividendDate', querySortConfig)}>配息基準日</th>
                      <th onClick={() => requestQuerySort('dividendType')} className={getSortClass('dividendType', querySortConfig)}>配息頻率</th>
                      <th onClick={() => requestQuerySort('nav')} className={`text-right ${getSortClass('nav', querySortConfig)}`}>NAV</th>
                      <th onClick={() => requestQuerySort('unit')} className={`text-right ${getSortClass('unit', querySortConfig)}`}>單位數</th>
                      <th onClick={() => requestQuerySort('divTot')} className={`text-right ${getSortClass('divTot', querySortConfig)}`}>總可分配金額</th>
                      <th onClick={() => requestQuerySort('divRate')} className={`text-right ${getSortClass('divRate', querySortConfig)}`}>配息率</th>
                    </tr>
                  </thead>
                  <tbody>
                    {sortedQueryDividends.map((div, index) => (
                      <tr key={index}>
                        <td>{div.fundNo}</td>
                        <td>{div.dividendYear || '-'}</td>
                        <td>{new Date(div.dividendDate).toLocaleDateString('zh-TW')}</td>
                        <td>{div.dividendType}</td>
                        <td className="text-right numeric">
                          {div.nav?.toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 8 }) || '-'}
                        </td>
                        <td className="text-right numeric">
                          {div.unit?.toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) || '-'}
                        </td>
                        <td className="text-right numeric">
                          {div.divTot?.toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) || '-'}
                        </td>
                        <td className="text-right numeric">
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

      {activeTab === 'composition' && (
        <>
          <section className="dividend-section">
            <h3>1. 匯入配息組成檔案 (CSV)</h3>
            <div className="form-group">
              <label style={{marginBottom: '10px'}}>選擇 CSV 檔案（包含收益分配比率與本金分配比率）：</label>
              
              <div className="file-input-wrapper" style={{ marginBottom: '15px' }}>
                <input
                  type="file"
                  accept=".csv"
                  onChange={handleCompositionFileChange}
                  disabled={uploading}
                  ref={compositionFileInputRef}
                />
                <button 
                  type="button" 
                  className="btn btn-secondary" 
                  onClick={triggerCompositionFileSelect}
                  disabled={uploading}
                >
                  {uploading ? '處理中...' : '選擇檔案'}
                </button>
                <span className="file-name">
                  {compositionFile ? compositionFile.name : '未選擇任何檔案'}
                </span>
              </div>
              
              <button 
                onClick={handleCompositionImport} 
                disabled={!compositionFile || uploading}
                className="btn btn-primary"
                style={{ width: '100%', maxWidth: '400px' }}
              >
                {uploading ? '匯入中...' : '匯入組成檔案'}
              </button>
            </div>

            {compositionImportResult && (
              <div className={`result-box ${compositionImportResult.success ? 'success' : 'error'}`}>
                <h4>匯入結果</h4>
                <p><strong>新增/更新：</strong>{compositionImportResult.updated} 筆</p>
                <p><strong>失敗：</strong>{compositionImportResult.failed} 筆</p>
                {compositionImportResult.errors.length > 0 && (
                  <div className="errors" style={{ marginTop: '10px' }}>
                    <strong>錯誤訊息：</strong>
                    <ul>
                      {compositionImportResult.errors.map((error, index) => (
                        <li key={index}>{error}</li>
                      ))}
                    </ul>
                  </div>
                )}
              </div>
            )}
          </section>

          <section className="dividend-section">
            <h3>2. 上傳 EC 官網</h3>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '15px', marginBottom: '15px' }}>
              <div className="form-group">
                <label>基金代號：</label>
                <input
                  type="text"
                  className="form-control"
                  value={compFundNo}
                  onChange={(e) => setCompFundNo(e.target.value)}
                  placeholder="例如：D109"
                  maxLength={20}
                />
              </div>
              <div className="form-group">
                <label>配息基準日：</label>
                <input
                  type="date"
                  className="form-control"
                  value={compStartDate}
                  onChange={(e) => setCompStartDate(e.target.value)}
                  max="9999-12-31"
                />
              </div>
              <div className="form-group" style={{ display: 'flex', alignItems: 'flex-end' }}>
                <button 
                  onClick={handleCompQuery} 
                  disabled={compLoading}
                  className="btn btn-primary"
                  style={{ width: '100%' }}
                >
                  {compLoading ? '查詢中...' : '查詢配息資料'}
                </button>
              </div>
            </div>

            {sortedCompDividends.length > 0 && (
              <div className="result-box success">
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '10px' }}>
                  <h4>查詢結果（共 {sortedCompDividends.length} 筆）</h4>
                  <button 
                    onClick={handleBatchUploadEc} 
                    disabled={uploadingToEc || selectedDividends.size === 0}
                    className="btn btn-success"
                  >
                    {uploadingToEc ? '上傳中...' : `上傳選取項目至 EC (${selectedDividends.size})`}
                  </button>
                </div>

                {ecUploadResult && (
                  <div className={`message ${ecUploadResult.failed === 0 ? 'success' : 'error'}`} style={{ marginBottom: '15px' }}>
                    <p>上傳結果：成功 {ecUploadResult.success} 筆，失敗 {ecUploadResult.failed} 筆</p>
                    {ecUploadResult.errors.length > 0 && (
                      <ul>
                        {ecUploadResult.errors.map((e, i) => <li key={i}>{e}</li>)}
                      </ul>
                    )}
                  </div>
                )}

                <div className="table-wrapper">
                  <table className="dividend-table selectable">
                    <thead>
                      <tr>
                        <th style={{ width: '40px', textAlign: 'center' }}>
                          {/* 移除全選 Checkbox */}
                        </th>
                        <th onClick={() => requestCompSort('fundNo')} className={getSortClass('fundNo', compSortConfig)}>基金代號</th>
                        <th onClick={() => requestCompSort('dividendDate')} className={getSortClass('dividendDate', compSortConfig)}>配息基準日</th>
                        <th onClick={() => requestCompSort('dividendType')} className={getSortClass('dividendType', compSortConfig)}>配息頻率</th>
                        <th onClick={() => requestCompSort('divRate')} className={`text-right ${getSortClass('divRate', compSortConfig)}`}>配息率</th>
                        <th onClick={() => requestCompSort('capitalRate')} className={`text-right ${getSortClass('capitalRate', compSortConfig)}`}>本金比率</th>
                        <th onClick={() => requestCompSort('interestRate')} className={`text-right ${getSortClass('interestRate', compSortConfig)}`}>收益比率</th>
                        <th onClick={() => requestCompSort('status')} className={`text-center ${getSortClass('status', compSortConfig)}`}>配息狀態</th>
                        <th onClick={() => requestCompSort('statusC')} className={`text-center ${getSortClass('statusC', compSortConfig)}`}>組成狀態</th>
                      </tr>
                    </thead>
                    <tbody>
                      {sortedCompDividends.map((div, index) => {
                        const key = `${div.fundNo}_${div.dividendDate}_${div.dividendType}`;
                        const isSelected = selectedDividends.has(key);
                        return (
                          <tr key={index} onClick={(e) => handleRowClick(e, key)} className={isSelected ? 'selected' : ''}>
                            <td style={{ textAlign: 'center' }}>
                              <input 
                                type="checkbox" 
                                checked={isSelected}
                                onChange={(e) => handleCheckboxChange(key, e.target.checked)}
                              />
                            </td>
                            <td>{div.fundNo}</td>
                            <td>{new Date(div.dividendDate).toLocaleDateString('zh-TW')}</td>
                            <td>{div.dividendType}</td>
                            <td className="text-right numeric">
                              {div.divRate?.toLocaleString('zh-TW', { minimumFractionDigits: 6, maximumFractionDigits: 6 }) || '-'}
                            </td>
                            <td className="text-right numeric">
                              {div.capitalRate !== undefined ? (div.capitalRate * 100).toFixed(2) + '%' : '-'}
                            </td>
                            <td className="text-right numeric">
                              {div.interestRate !== undefined ? (div.interestRate * 100).toFixed(2) + '%' : '-'}
                            </td>
                            <td className="text-center">
                              {div.status === 'O' ? <span style={{color:'green'}}>已上傳</span> : <span style={{color:'orange'}}>未上傳</span>}
                            </td>
                            <td className="text-center">
                              {div.statusC === 'O' ? <span style={{color:'green'}}>已上傳</span> : <span style={{color:'orange'}}>未上傳</span>}
                            </td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </table>
                </div>
              </div>
            )}
          </section>
        </>
      )}
    </div>
  );
}
