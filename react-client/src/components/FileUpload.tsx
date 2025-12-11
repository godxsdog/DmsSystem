import { useState, useRef } from 'react';
import { apiClient } from '../api/client';

interface FileUploadProps {
  endpoint: string;
  title: string;
  description: string;
  onUploadSuccess?: (message: string, rowsAdded?: number) => void;
  onUploadError?: (error: string) => void;
}

export function FileUpload({
  endpoint,
  title,
  description,
  onUploadSuccess,
  onUploadError,
}: FileUploadProps) {
  const [file, setFile] = useState<File | null>(null);
  const [uploading, setUploading] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setFile(e.target.files[0]);
      setMessage(null);
    }
  };

  const handleUpload = async () => {
    if (!file) {
      setMessage('請選擇檔案');
      return;
    }

    setUploading(true);
    setMessage(null);

    try {
      const result = await apiClient.uploadFile(endpoint, file);
      setMessage(`成功: ${result.message}`);
      onUploadSuccess?.(result.message, result.rowsAdded);
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : '上傳失敗';
      setMessage(`錯誤: ${errorMessage}`);
      onUploadError?.(errorMessage);
    } finally {
      setUploading(false);
    }
  };

  const triggerFileSelect = () => {
    fileInputRef.current?.click();
  };

  return (
    <div className="file-upload-container">
      <h3>{title}</h3>
      <p>{description}</p>
      <div className="upload-controls">
        <div className="file-input-wrapper">
          <input
            type="file"
            accept=".xlsx,.csv"
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
          onClick={handleUpload} 
          disabled={!file || uploading}
          className="btn btn-primary"
          style={{ width: '100%' }}
        >
          {uploading ? '上傳中...' : '開始上傳'}
        </button>
      </div>
      {message && (
        <div className={`message ${message.startsWith('成功') ? 'success' : 'error'}`}>
          {message}
        </div>
      )}
    </div>
  );
}
