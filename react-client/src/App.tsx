import { useState } from 'react';
import { FileUpload } from './components/FileUpload';
import { DataView } from './pages/DataView';
import { Dividend } from './pages/Dividend';
import { Settings } from './pages/Settings';
import './App.css';

function App() {
  const [notifications, setNotifications] = useState<Array<{ type: 'success' | 'error'; message: string }>>([]);

  const addNotification = (type: 'success' | 'error', message: string) => {
    setNotifications([...notifications, { type, message }]);
    setTimeout(() => {
      setNotifications((prev) => prev.slice(1));
    }, 5000);
  };

  const [currentView, setCurrentView] = useState<'dividend' | 'shareholder' | 'settings'>('dividend');
  const [shareholderSubView, setShareholderSubView] = useState<'upload' | 'data'>('upload');

  return (
    <div className="app">
      <header className="app-header">
        <h1>DMS 系統</h1>
        <nav>
          <button
            className={currentView === 'dividend' ? 'active' : ''}
            onClick={() => setCurrentView('dividend')}
          >
            配息管理
          </button>
          <button
            className={currentView === 'shareholder' ? 'active' : ''}
            onClick={() => setCurrentView('shareholder')}
          >
            股東會相關資訊
          </button>
          <button
            className={currentView === 'settings' ? 'active' : ''}
            onClick={() => setCurrentView('settings')}
          >
            API連線設定
          </button>
        </nav>
      </header>
      <main className="app-main">
        {currentView === 'dividend' && <Dividend />}
        
        {currentView === 'shareholder' && (
          <div className="shareholder-container">
            <div className="tab-nav" style={{ marginBottom: '20px' }}>
              <button
                className={`tab-btn ${shareholderSubView === 'upload' ? 'active' : ''}`}
                onClick={() => setShareholderSubView('upload')}
              >
                檔案上傳
              </button>
              <button
                className={`tab-btn ${shareholderSubView === 'data' ? 'active' : ''}`}
                onClick={() => setShareholderSubView('data')}
              >
                資料檢視
              </button>
            </div>

            {shareholderSubView === 'upload' && (
              <>
                <div className="upload-sections">
                  <FileUpload
                    endpoint="/api/ShareholderMeetings/upload-shmtsource1"
                    title="股東會明細上傳"
                    description="上傳股東會明細資料（支援 .xlsx 和 .csv 格式）"
                    onUploadSuccess={(message) => addNotification('success', message)}
                    onUploadError={(error) => addNotification('error', error)}
                  />
                  <FileUpload
                    endpoint="/api/CompanyInfo/upload-shmtsource4"
                    title="公司資訊上傳"
                    description="上傳公司基本資訊（支援 .xlsx 和 .csv 格式）"
                    onUploadSuccess={(message) => addNotification('success', message)}
                    onUploadError={(error) => addNotification('error', error)}
                  />
                  <FileUpload
                    endpoint="/api/StockBalance/upload"
                    title="股票餘額上傳"
                    description="上傳股票餘額資料（支援 .csv 格式）"
                    onUploadSuccess={(message) => addNotification('success', message)}
                    onUploadError={(error) => addNotification('error', error)}
                  />
                </div>
                {notifications.length > 0 && (
                  <div className="notifications">
                    {notifications.map((notif, index) => (
                      <div key={index} className={`notification ${notif.type}`}>
                        {notif.message}
                      </div>
                    ))}
                  </div>
                )}
              </>
            )}
            
            {shareholderSubView === 'data' && <DataView />}
          </div>
        )}

        {currentView === 'settings' && <Settings />}
      </main>
    </div>
  );
}

export default App;
