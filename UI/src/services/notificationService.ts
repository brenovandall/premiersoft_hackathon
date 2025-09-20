// Serviço para notificações de upload
export interface NotificationPayload {
  success: boolean;
  key: string;
  fileName: string;
  size: number;
  url: string;
  dataType: string;
  fileFormat: string;
  description?: string;
  uploadTimestamp: string;
  error?: string;
}

export interface NotificationService {
  notifyUploadSuccess(payload: NotificationPayload): Promise<void>;
  notifyUploadFailure(payload: NotificationPayload): Promise<void>;
}

// Implementação para API REST
class APINotificationService implements NotificationService {
  private baseUrl: string;
  private successEndpoint: string;
  private failureEndpoint: string;

  constructor() {
    this.baseUrl = import.meta.env.VITE_API_BASE_URL || '';
    this.successEndpoint = import.meta.env.VITE_SUCCESS_NOTIFICATION_ENDPOINT || '/file-upload/success';
    this.failureEndpoint = import.meta.env.VITE_FAILURE_NOTIFICATION_ENDPOINT || '/file-upload/failure';
  }

  async notifyUploadSuccess(payload: NotificationPayload): Promise<void> {
    if (!this.baseUrl) {
      console.warn('⚠️ API base URL not configured for upload success notification');
      return;
    }

    try {
      const response = await fetch(`${this.baseUrl}${this.successEndpoint}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        },
        body: JSON.stringify({
          ...payload,
          status: 'success',
          message: 'File uploaded successfully to S3',
          timestamp: new Date().toISOString(),
        }),
      });

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
      }

      const responseData = await response.json();
      console.log('✅ Upload success notification sent:', responseData);
    } catch (error) {
      console.error('❌ Failed to notify upload success:', error);
      throw error;
    }
  }

  async notifyUploadFailure(payload: NotificationPayload): Promise<void> {
    if (!this.baseUrl) {
      console.warn('⚠️ API base URL not configured for upload failure notification');
      return;
    }

    try {
      const response = await fetch(`${this.baseUrl}${this.failureEndpoint}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        },
        body: JSON.stringify({
          ...payload,
          status: 'failure',
          message: 'File upload failed',
          timestamp: new Date().toISOString(),
        }),
      });

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
      }

      const responseData = await response.json();
      console.log('📋 Upload failure notification sent:', responseData);
    } catch (error) {
      console.error('❌ Failed to notify upload failure:', error);
      // Não relança o erro para falhas de notificação
    }
  }
}

// Implementação para Webhook
class WebhookNotificationService implements NotificationService {
  private webhookUrl: string;

  constructor() {
    this.webhookUrl = import.meta.env.VITE_WEBHOOK_URL || '';
  }

  async notifyUploadSuccess(payload: NotificationPayload): Promise<void> {
    if (!this.webhookUrl) {
      console.warn('⚠️ Webhook URL not configured');
      return;
    }

    try {
      const response = await fetch(this.webhookUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          event: 'file.upload.success',
          timestamp: new Date().toISOString(),
          data: payload,
        }),
      });

      if (!response.ok) {
        throw new Error(`Webhook notification failed: ${response.status}`);
      }

      console.log('✅ Webhook success notification sent');
    } catch (error) {
      console.error('❌ Failed to send webhook notification:', error);
      throw error;
    }
  }

  async notifyUploadFailure(payload: NotificationPayload): Promise<void> {
    if (!this.webhookUrl) {
      console.warn('⚠️ Webhook URL not configured');
      return;
    }

    try {
      const response = await fetch(this.webhookUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          event: 'file.upload.failure',
          timestamp: new Date().toISOString(),
          data: payload,
        }),
      });

      if (!response.ok) {
        throw new Error(`Webhook notification failed: ${response.status}`);
      }

      console.log('📋 Webhook failure notification sent');
    } catch (error) {
      console.error('❌ Failed to send webhook failure notification:', error);
    }
  }
}

// Factory para criar o serviço apropriado
export function createNotificationService(): NotificationService {
  const notificationType = import.meta.env.VITE_NOTIFICATION_TYPE || 'api';
  
  switch (notificationType.toLowerCase()) {
    case 'webhook':
      return new WebhookNotificationService();
    case 'api':
    default:
      return new APINotificationService();
  }
}

// Instância padrão do serviço
export const notificationService = createNotificationService();

// Hook para usar o serviço de notificação
export function useNotificationService() {
  const service = createNotificationService();
  
  const notifySuccess = async (payload: NotificationPayload): Promise<boolean> => {
    try {
      await service.notifyUploadSuccess(payload);
      return true;
    } catch (error) {
      console.error('Failed to send success notification:', error);
      return false;
    }
  };

  const notifyFailure = async (payload: NotificationPayload): Promise<boolean> => {
    try {
      await service.notifyUploadFailure(payload);
      return true;
    } catch (error) {
      console.error('Failed to send failure notification:', error);
      return false;
    }
  };

  return {
    notifySuccess,
    notifyFailure,
  };
}