import { BackendProcessingData } from '@/types/import';

// Interface para resposta do backend
export interface BackendProcessingResponse {
  success: boolean;
  message: string;
  data?: {
    processingId: string;
    estimatedTime?: number;
    queuePosition?: number;
  };
  error?: string;
}

// Configuração da API do backend
const API_CONFIG = {
  baseUrl: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api',
  timeout: 30000, // 30 segundos
};

/**
 * Envia os dados do arquivo e mapeamentos para o backend processar
 */
export async function sendFileToBackend(data: BackendProcessingData): Promise<BackendProcessingResponse> {
  try {
    const response = await fetch(`${API_CONFIG.baseUrl}/import/process`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
      body: JSON.stringify(data),
      signal: AbortSignal.timeout(API_CONFIG.timeout),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || `Erro HTTP: ${response.status} ${response.statusText}`);
    }

    const result = await response.json();
    
    return {
      success: true,
      message: result.message || 'Arquivo enviado para processamento com sucesso',
      data: result.data,
    };

  } catch (error) {
    console.error('Erro ao enviar arquivo para o backend:', error);

    if (error instanceof Error) {
      if (error.name === 'TimeoutError') {
        return {
          success: false,
          message: 'Timeout na comunicação com o backend',
          error: 'A requisição demorou mais que o esperado para ser processada',
        };
      }

      if (error.message.includes('fetch')) {
        return {
          success: false,
          message: 'Erro de conectividade',
          error: 'Não foi possível conectar ao backend. Verifique se o serviço está rodando.',
        };
      }

      return {
        success: false,
        message: 'Erro no processamento',
        error: error.message,
      };
    }

    return {
      success: false,
      message: 'Erro desconhecido',
      error: 'Ocorreu um erro inesperado durante o processamento',
    };
  }
}

/**
 * Verifica o status de processamento de um arquivo
 */
export async function checkProcessingStatus(processingId: string): Promise<{
  success: boolean;
  data?: {
    status: 'pending' | 'processing' | 'completed' | 'error';
    progress?: number;
    message?: string;
    result?: any;
  };
  error?: string;
}> {
  try {
    const response = await fetch(`${API_CONFIG.baseUrl}/import/status/${processingId}`, {
      method: 'GET',
      headers: {
        'Accept': 'application/json',
      },
      signal: AbortSignal.timeout(10000), // 10 segundos para status
    });

    if (!response.ok) {
      throw new Error(`Erro HTTP: ${response.status}`);
    }

    const result = await response.json();
    
    return {
      success: true,
      data: result.data,
    };

  } catch (error) {
    console.error('Erro ao verificar status:', error);
    return {
      success: false,
      error: error instanceof Error ? error.message : 'Erro desconhecido',
    };
  }
}

/**
 * Testa a conectividade com o backend
 */
export async function testBackendConnection(): Promise<{ success: boolean; message: string }> {
  try {
    const response = await fetch(`${API_CONFIG.baseUrl}/health`, {
      method: 'GET',
      signal: AbortSignal.timeout(5000),
    });

    if (response.ok) {
      return { success: true, message: 'Backend conectado com sucesso' };
    } else {
      return { success: false, message: `Backend respondeu com status ${response.status}` };
    }
  } catch (error) {
    return { 
      success: false, 
      message: error instanceof Error ? error.message : 'Erro de conectividade' 
    };
  }
}