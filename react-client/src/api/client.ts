const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5137';

export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
}

export class ApiClient {
  private baseUrl: string;

  constructor(baseUrl: string = API_BASE_URL) {
    this.baseUrl = baseUrl;
  }

  async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const url = `${this.baseUrl}${endpoint}`;
    const response = await fetch(url, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      },
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: response.statusText }));
      throw new Error(error.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  }

  async uploadFile(
    endpoint: string,
    file: File
  ): Promise<{ success: boolean; message: string; rowsAdded?: number }> {
    const formData = new FormData();
    formData.append('file', file);

    const url = `${this.baseUrl}${endpoint}`;
    const response = await fetch(url, {
      method: 'POST',
      body: formData,
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: response.statusText }));
      throw new Error(error.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  }

  // 股東會明細上傳
  async uploadShareholderMeetingDetails(file: File) {
    return this.uploadFile('/api/ShareholderMeetings/upload-shmtsource1', file);
  }

  // 公司資訊上傳
  async uploadCompanyInfo(file: File) {
    return this.uploadFile('/api/CompanyInfo/upload-shmtsource4', file);
  }

  // 股票餘額上傳
  async uploadStockBalance(file: File) {
    return this.uploadFile('/api/StockBalance/upload', file);
  }
}

export const apiClient = new ApiClient();

