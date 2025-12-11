import { useState, useEffect } from 'react';
import { apiClient } from '../api/client';

export function Settings() {
  const [apiBaseUrl, setApiBaseUrl] = useState(apiClient.getBaseUrl());

  const handleApiUrlChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newUrl = e.target.value;
    setApiBaseUrl(newUrl);
    apiClient.setBaseUrl(newUrl);
  };

  const setUrl = (url: string) => {
    setApiBaseUrl(url);
    apiClient.setBaseUrl(url);
  };

  return (
    <div className="settings-container" style={{ padding: '20px', maxWidth: '800px', margin: '0 auto' }}>
      <h2>API 連線設定</h2>
      
      <div className="form-group" style={{ marginBottom: '20px' }}>
        <label style={{ display: 'block', marginBottom: '8px', fontWeight: 'bold' }}>後端 API 網址：</label>
        <input
          type="text"
          value={apiBaseUrl}
          onChange={handleApiUrlChange}
          placeholder="例如 http://localhost:5137"
          style={{ width: '100%', padding: '10px', marginBottom: '10px', fontSize: '16px', borderRadius: '4px', border: '1px solid #ccc' }}
        />
        <p style={{ fontSize: '0.9em', color: '#666' }}>
          目前連線網址: {apiBaseUrl}
        </p>
      </div>

      <div style={{ backgroundColor: '#f8f9fa', padding: '20px', borderRadius: '8px', border: '1px solid #dee2e6' }}>
        <h3 style={{ marginTop: '0', marginBottom: '15px' }}>常見連接埠說明</h3>
        <p>這些是 Visual Studio 與 IIS Express 的預設連接埠，請依據您的後端啟動方式選擇：</p>
        
        <ul style={{ paddingLeft: '20px', lineHeight: '1.8', listStyle: 'none' }}>
          <li style={{ marginBottom: '8px' }}>
            <button 
              onClick={() => setUrl('http://localhost:5137')}
              className="btn btn-outline"
              style={{ marginRight: '10px', padding: '0.25rem 0.5rem', fontSize: '0.9rem' }}
            >
              http://localhost:5137
            </button> 
            <span style={{ color: '#555' }}>- VS2022 預設 HTTP</span>
          </li>
          <li style={{ marginBottom: '8px' }}>
            <button 
              onClick={() => setUrl('https://localhost:7036')}
              className="btn btn-outline"
              style={{ marginRight: '10px', padding: '0.25rem 0.5rem', fontSize: '0.9rem' }}
            >
              https://localhost:7036
            </button> 
            <span style={{ color: '#555' }}>- VS2022 預設 HTTPS</span>
          </li>
          <li style={{ marginBottom: '8px' }}>
            <button 
              onClick={() => setUrl('http://localhost:35912')}
              className="btn btn-outline"
              style={{ marginRight: '10px', padding: '0.25rem 0.5rem', fontSize: '0.9rem' }}
            >
              http://localhost:35912
            </button> 
            <span style={{ color: '#555' }}>- IIS Express HTTP</span>
          </li>
          <li style={{ marginBottom: '8px' }}>
            <button 
              onClick={() => setUrl('https://localhost:44301')}
              className="btn btn-outline"
              style={{ marginRight: '10px', padding: '0.25rem 0.5rem', fontSize: '0.9rem' }}
            >
              https://localhost:44301
            </button> 
            <span style={{ color: '#555' }}>- IIS Express HTTPS</span>
          </li>
        </ul>

        <div style={{ marginTop: '15px', padding: '10px', backgroundColor: '#fff3cd', borderRadius: '4px', border: '1px solid #ffeeba' }}>
          <strong style={{ color: '#856404' }}>⚠ 注意：</strong> 
          <span style={{ color: '#856404' }}> 若使用 HTTPS，瀏覽器可能會因為自簽名憑證阻擋請求。請先在瀏覽器新分頁開啟 API 網址 (如 <a href={`${apiBaseUrl.replace('http:', 'https:')}/weatherforecast`} target="_blank" rel="noreferrer">/weatherforecast</a>) 並接受風險。</span>
        </div>
      </div>
    </div>
  );
}
