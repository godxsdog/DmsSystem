import { useState, useEffect } from 'react';
import { apiClient } from '../api/client';
import './DataView.css';

interface ShareholderMeeting {
  acDate: string;
  empNo: string;
  stkCd: string;
  stkName: string;
  shmtDate: string;
  type: string;
  shmtAddr: string;
}

interface CompanyInfo {
  acDate: string;
  empNo: string;
  stkCd: string;
  stkName: string;
  compName: string;
  tel: string;
  addr: string;
  spokesman: string;
  president: string;
  chairman: string;
}

interface StockBalance {
  id: string;
  contractSeq: string;
  acDate: string;
  stockNo: string;
  shares: number;
}

export function DataView() {
  const [activeTab, setActiveTab] = useState<'meetings' | 'companies' | 'balance'>('meetings');
  const [meetings, setMeetings] = useState<ShareholderMeeting[]>([]);
  const [companies, setCompanies] = useState<CompanyInfo[]>([]);
  const [balance, setBalance] = useState<StockBalance[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadData();
  }, [activeTab]);

  const loadData = async () => {
    setLoading(true);
    setError(null);
    try {
      if (activeTab === 'meetings') {
        const response = await apiClient.request<{ data: ShareholderMeeting[] }>('/api/DataView/shareholder-meetings');
        setMeetings(response.data || []);
      } else if (activeTab === 'companies') {
        const response = await apiClient.request<{ data: CompanyInfo[] }>('/api/DataView/company-info');
        setCompanies(response.data || []);
      } else if (activeTab === 'balance') {
        const response = await apiClient.request<{ data: StockBalance[] }>('/api/DataView/stock-balance');
        setBalance(response.data || []);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : '載入資料失敗');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="data-view">
      <h2>資料檢視</h2>
      <div className="tabs">
        <button
          className={activeTab === 'meetings' ? 'active' : ''}
          onClick={() => setActiveTab('meetings')}
        >
          股東會明細
        </button>
        <button
          className={activeTab === 'companies' ? 'active' : ''}
          onClick={() => setActiveTab('companies')}
        >
          公司資訊
        </button>
        <button
          className={activeTab === 'balance' ? 'active' : ''}
          onClick={() => setActiveTab('balance')}
        >
          股票餘額
        </button>
      </div>

      {loading && <div className="loading">載入中...</div>}
      {error && <div className="error">錯誤: {error}</div>}

      {activeTab === 'meetings' && (
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>股票代號</th>
                <th>股票名稱</th>
                <th>股東會日期</th>
                <th>類型</th>
                <th>開會地點</th>
                <th>處理日期</th>
              </tr>
            </thead>
            <tbody>
              {meetings.map((item, index) => (
                <tr key={index}>
                  <td>{item.stkCd}</td>
                  <td>{item.stkName}</td>
                  <td>{item.shmtDate}</td>
                  <td>{item.type}</td>
                  <td>{item.shmtAddr}</td>
                  <td>{item.acDate}</td>
                </tr>
              ))}
            </tbody>
          </table>
          {meetings.length === 0 && !loading && <p>尚無資料</p>}
        </div>
      )}

      {activeTab === 'companies' && (
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>股票代號</th>
                <th>股票名稱</th>
                <th>公司名稱</th>
                <th>電話</th>
                <th>地址</th>
                <th>董事長</th>
                <th>總經理</th>
              </tr>
            </thead>
            <tbody>
              {companies.map((item, index) => (
                <tr key={index}>
                  <td>{item.stkCd}</td>
                  <td>{item.stkName}</td>
                  <td>{item.compName}</td>
                  <td>{item.tel}</td>
                  <td>{item.addr}</td>
                  <td>{item.chairman}</td>
                  <td>{item.president}</td>
                </tr>
              ))}
            </tbody>
          </table>
          {companies.length === 0 && !loading && <p>尚無資料</p>}
        </div>
      )}

      {activeTab === 'balance' && (
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>基金代碼</th>
                <th>契約序號</th>
                <th>記帳日</th>
                <th>股票代號</th>
                <th>庫存數量</th>
              </tr>
            </thead>
            <tbody>
              {balance.map((item, index) => (
                <tr key={index}>
                  <td>{item.id}</td>
                  <td>{item.contractSeq}</td>
                  <td>{item.acDate}</td>
                  <td>{item.stockNo}</td>
                  <td>{item.shares}</td>
                </tr>
              ))}
            </tbody>
          </table>
          {balance.length === 0 && !loading && <p>尚無資料</p>}
        </div>
      )}
    </div>
  );
}

