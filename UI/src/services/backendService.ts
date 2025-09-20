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
  baseUrl: import.meta.env.VITE_API_BASE_URL || 'http://localhost:50915/v1/CsvFileRead',
  timeout: 30000, // 30 segundos
};

/**
 * Envia os dados do arquivo e mapeamentos para o backend processar
 */
export async function sendFileToBackend(data: BackendProcessingData): Promise<BackendProcessingResponse> {
  try {
    // Dados que serão enviados para o backend
    console.log('📤 Enviando dados para o backend via POST:');
    console.log('� URL:', `${API_CONFIG.baseUrl}/upload`);
    console.log('📊 Dados:', data);

    // Chamada real para o backend
    const response = await fetch(`${API_CONFIG.baseUrl}/upload`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`HTTP ${response.status}: ${errorText}`);
    }

    const result = await response.json();
    console.log('✅ Resposta do backend:', result);
    
    return {
      success: result.success || true,
      message: result.message || 'Dados enviados com sucesso para o backend',
      data: {
        processingId: result.data?.ProcessingId || `backend_${Date.now()}`,
        estimatedTime: 30,
      },
    };

  } catch (error) {
    console.error('❌ Erro ao enviar dados para o backend:', error);

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
    // Simular verificação de status
    console.log(`🔍 Verificando status do processamento: ${processingId}`);
    
    // Simular delay
    await new Promise(resolve => setTimeout(resolve, 500));
    
    // Simular resultado baseado no ID
    const isCompleted = processingId.startsWith('sim_');
    
    return {
      success: true,
      data: {
        status: isCompleted ? 'completed' : 'processing',
        progress: isCompleted ? 100 : 75,
        message: isCompleted ? 'Processamento concluído com sucesso' : 'Processamento em andamento',
        result: isCompleted ? {
          recordsProcessed: 150,
          recordsSuccess: 145,
          recordsError: 5,
          processingTime: '00:00:25'
        } : undefined
      },
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
    const response = await fetch(`${API_CONFIG.baseUrl}/ok`, {
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