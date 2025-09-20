import { useState } from 'react';
import { PutObjectCommand } from '@aws-sdk/client-s3';
import { s3Client, S3_CONFIG, generateS3Key, getS3PublicUrl, validateS3Config } from '@/lib/s3Client';

interface UploadProgress {
  loaded: number;
  total: number;
  percentage: number;
}

interface UploadResult {
  success: boolean;
  message: string;
  data?: {
    key: string;
    fileName: string;
    size: number;
    url: string;
    bucket: string;
  };
  error?: string;
}

interface UploadMetadata {
  dataType: string;
  fileFormat: string;
  description?: string;
  originalFileName: string;
  fileSize: number;
  uploadTimestamp: string;
}

export const useS3Upload = () => {
  const [isUploading, setIsUploading] = useState(false);
  const [progress, setProgress] = useState<UploadProgress | null>(null);
  const [error, setError] = useState<string | null>(null);

  const uploadFile = async (
    file: File,
    dataType: string,
    fileFormat: string,
    description?: string
  ): Promise<UploadResult> => {
    setIsUploading(true);
    setError(null);
    setProgress({ loaded: 0, total: file.size, percentage: 0 });

    try {
      // Validar configuração S3
      const configValidation = validateS3Config();
      if (!configValidation.isValid) {
        throw new Error(`Configuração S3 incompleta. Variáveis faltando: ${configValidation.missingVars.join(', ')}`);
      }

      // Validações do arquivo
      if (!file) {
        throw new Error('Nenhum arquivo foi fornecido');
      }

      // Verificar se o arquivo ainda é acessível
      if (!file.size || file.size === 0) {
        throw new Error('Arquivo vazio ou inacessível. Tente selecionar o arquivo novamente.');
      }

      if (file.size > S3_CONFIG.maxFileSize) {
        throw new Error(`Arquivo muito grande. Máximo permitido: ${S3_CONFIG.maxFileSize / 1024 / 1024}MB`);
      }

      if (!S3_CONFIG.bucketName) {
        throw new Error('Nome do bucket S3 não configurado');
      }

      // Gerar chave única para o arquivo
      const key = generateS3Key(dataType, file.name);
      
      // Simular progresso do upload
      const progressInterval = setInterval(() => {
        setProgress(prev => {
          if (!prev) return null;
          const increment = Math.min(file.size * 0.1, prev.total - prev.loaded);
          const newLoaded = prev.loaded + increment;
          return {
            loaded: newLoaded,
            total: file.size,
            percentage: Math.min((newLoaded / file.size) * 100, 95), // Máximo 95% até completar
          };
        });
      }, 300);

      // Converter arquivo para formato compatível com S3
      let fileBody: ArrayBuffer;
      try {
        fileBody = await file.arrayBuffer();
      } catch (arrayBufferError) {
        // Fallback: usar FileReader se arrayBuffer falhar
        fileBody = await new Promise<ArrayBuffer>((resolve, reject) => {
          const reader = new FileReader();
          reader.onload = () => resolve(reader.result as ArrayBuffer);
          reader.onerror = () => reject(new Error('Erro ao ler arquivo com FileReader'));
          reader.readAsArrayBuffer(file);
        });
      }

      // Preparar metadados
      const metadata: Record<string, string> = {
        'original-filename': file.name,
        'data-type': dataType,
        'file-format': fileFormat,
        'description': description || '',
        'upload-date': new Date().toISOString(),
        'file-size': file.size.toString(),
        'content-type': file.type || 'application/octet-stream',
      };

      // Preparar comando de upload - usar ArrayBuffer
      const uploadCommand = new PutObjectCommand({
        Bucket: S3_CONFIG.bucketName,
        Key: key,
        Body: fileBody,
        ContentType: file.type || 'application/octet-stream',
        Metadata: metadata,
      });

      // Fazer upload para S3
      const uploadResponse = await s3Client.send(uploadCommand);
      
      clearInterval(progressInterval);
      setProgress({ loaded: file.size, total: file.size, percentage: 100 });

      const fileUrl = getS3PublicUrl(key);
      
      const result: UploadResult = {
        success: true,
        message: 'Arquivo enviado com sucesso para S3',
        data: {
          key,
          fileName: file.name,
          size: file.size,
          url: fileUrl,
          bucket: S3_CONFIG.bucketName,
        },
      };

      // Opcional: Notificar API externa sobre o sucesso do upload
      // await notifyExternalAPI({
      //   success: true,
      //   key,
      //   fileName: file.name,
      //   size: file.size,
      //   url: fileUrl,
      //   dataType,
      //   fileFormat,
      //   description,
      //   uploadTimestamp: new Date().toISOString(),
      // });

      return result;

    } catch (error) {
      console.error('❌ Erro no upload S3:', error);
      clearInterval(progressInterval);
      
      let errorMessage = 'Erro desconhecido no upload';
      
      if (error instanceof Error) {
        // Tratar diferentes tipos de erro
        if (error.name === 'NotReadableError') {
          errorMessage = 'Erro ao ler o arquivo. Tente selecionar o arquivo novamente.';
        } else if (error.message.includes('readableStream.getReader is not a function')) {
          errorMessage = 'Erro de compatibilidade de stream. Arquivo pode estar corrompido.';
        } else if (error.message.includes('NetworkingError')) {
          errorMessage = 'Erro de conexão. Verifique sua internet e tente novamente.';
        } else if (error.message.includes('AccessDenied')) {
          errorMessage = 'Erro de permissão. Verifique as credenciais AWS.';
        } else {
          errorMessage = error.message;
        }
      }
      
      setError(errorMessage);

      // Opcional: Notificar API externa sobre a falha do upload
      // await notifyExternalAPI({
      //   success: false,
      //   key: '',
      //   fileName: file.name,
      //   size: file.size,
      //   url: '',
      //   dataType,
      //   fileFormat,
      //   description,
      //   uploadTimestamp: new Date().toISOString(),
      //   error: errorMessage,
      // });

      return {
        success: false,
        message: 'Erro no upload para S3',
        error: errorMessage,
      };
    } finally {
      setIsUploading(false);
      setTimeout(() => {
        setProgress(null);
        setError(null);
      }, 3000);
    }
  };

  const notifyExternalAPI = async (payload: any): Promise<void> => {
    const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;
    const endpoint = payload.success 
      ? import.meta.env.VITE_SUCCESS_NOTIFICATION_ENDPOINT 
      : import.meta.env.VITE_FAILURE_NOTIFICATION_ENDPOINT;
    
    if (!apiBaseUrl || !endpoint) {
      console.log('ℹ️ API notification endpoints not configured - skipping notification');
      return;
    }

    try {
      const response = await fetch(`${apiBaseUrl}${endpoint}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(payload),
      });

      if (response.ok) {
        console.log('✅ API notified successfully');
      } else {
        console.warn(`⚠️ API notification failed: ${response.status}`);
      }
    } catch (error) {
      console.warn('⚠️ Failed to notify API:', error);
    }
  };

  return {
    uploadFile,
    isUploading,
    progress,
    error,
  };
};