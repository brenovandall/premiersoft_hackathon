import { useState } from 'react';
import { 
  PutObjectCommand, 
  CreateMultipartUploadCommand,
  UploadPartCommand,
  CompleteMultipartUploadCommand,
  AbortMultipartUploadCommand 
} from '@aws-sdk/client-s3';
import { getSignedUrl } from '@aws-sdk/s3-request-presigner';
import { s3Client, S3_CONFIG, generateS3Key, getS3PublicUrl, validateS3Config } from '@/lib/s3Client';

interface UploadProgress {
  loaded: number;
  total: number;
  percentage: number;
  currentPart?: number;
  totalParts?: number;
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
    uploadId?: string;
    etag?: string;
  };
  error?: string;
}

interface MultipartPart {
  PartNumber: number;
  ETag: string;
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

      // Validar formatos de arquivo permitidos
      const fileName = file.name.toLowerCase();
      const allowedExtensions = ['.csv', '.xml', '.json', '.xlsx', '.xls'];
      const hasValidExtension = allowedExtensions.some(ext => fileName.endsWith(ext));
      
      if (!hasValidExtension) {
        throw new Error(`Formato de arquivo não suportado. Formatos permitidos: ${allowedExtensions.join(', ')}`);
      }

      if (file.size > S3_CONFIG.maxFileSize) {
        throw new Error(`Arquivo muito grande. Máximo permitido: ${S3_CONFIG.maxFileSize / 1024 / 1024}MB`);
      }

      // Aviso especial para arquivos muito grandes
      if (file.size > 3 * 1024 * 1024 * 1024) { // 3GB
        console.warn(`⚠️ Arquivo muito grande (${(file.size / 1024 / 1024 / 1024).toFixed(1)}GB). Upload pode ser lento mas é suportado até 5GB.`);
      }

      // Validar se o formato do arquivo corresponde ao formato selecionado
      const validateFileFormat = (fileName: string, selectedFormat: string): boolean => {
        const lowerFileName = fileName.toLowerCase();
        switch (selectedFormat) {
          case 'csv':
            return lowerFileName.endsWith('.csv') || lowerFileName.endsWith('.xlsx') || lowerFileName.endsWith('.xls');
          case 'xml':
            return lowerFileName.endsWith('.xml');
          case 'json':
            return lowerFileName.endsWith('.json');
          default:
            return false;
        }
      };

      if (!validateFileFormat(file.name, fileFormat)) {
        throw new Error(`Arquivo não compatível com o formato selecionado (${fileFormat.toUpperCase()}). Verifique se selecionou o formato correto.`);
      }

      if (!S3_CONFIG.bucketName) {
        throw new Error('Nome do bucket S3 não configurado');
      }

      // Gerar chave única para o arquivo
      const key = generateS3Key(dataType, file.name);
      
      // Simular progresso do upload (mais lento para arquivos grandes)
      const isLargeFile = file.size > 100 * 1024 * 1024; // 100MB
      const progressInterval = setInterval(() => {
        setProgress(prev => {
          if (!prev) return null;
          // Progresso mais lento para arquivos grandes
          const incrementRate = isLargeFile ? 0.02 : 0.1; // 2% vs 10%
          const increment = Math.min(file.size * incrementRate, prev.total - prev.loaded);
          const newLoaded = prev.loaded + increment;
          return {
            loaded: newLoaded,
            total: file.size,
            percentage: Math.min((newLoaded / file.size) * 100, 90), // Máximo 90% até completar
          };
        });
      }, isLargeFile ? 1000 : 300); // Update a cada 1s para arquivos grandes

      // Converter arquivo para formato compatível com S3
      let fileBody: ArrayBuffer;
      
      // Para arquivos muito grandes (>4GB), usar abordagem especial
      if (file.size > 4 * 1024 * 1024 * 1024) { // 4GB
        console.log('🔄 Arquivo muito grande (>4GB), processando em modo otimizado...');
        
        try {
          // Tentar usar stream diretamente para arquivos muito grandes
          fileBody = await new Promise<ArrayBuffer>((resolve, reject) => {
            const reader = new FileReader();
            
            reader.onloadstart = () => {
              console.log('📖 Iniciando leitura do arquivo grande...');
            };
            
            reader.onprogress = (event) => {
              if (event.lengthComputable) {
                const percentComplete = (event.loaded / event.total) * 100;
                console.log(`📖 Lendo arquivo: ${percentComplete.toFixed(1)}%`);
              }
            };
            
            reader.onload = () => {
              console.log('✅ Leitura do arquivo concluída');
              resolve(reader.result as ArrayBuffer);
            };
            
            reader.onerror = () => {
              console.error('❌ Erro na leitura:', reader.error);
              reject(new Error(`Erro ao ler arquivo grande: ${reader.error?.message || 'Erro desconhecido'}`));
            };
            
            reader.onabort = () => {
              reject(new Error('Leitura do arquivo foi cancelada'));
            };
            
            // Usar readAsArrayBuffer com timeout aumentado
            setTimeout(() => {
              try {
                reader.readAsArrayBuffer(file);
              } catch (readError) {
                reject(new Error(`Não foi possível iniciar a leitura do arquivo: ${readError}`));
              }
            }, 100);
          });
        } catch (largeFileError) {
          throw new Error(`Arquivo muito grande para processar no navegador (${(file.size / 1024 / 1024 / 1024).toFixed(1)}GB). Limite máximo: 5GB. Considere dividir o arquivo em partes menores.`);
        }
      } else {
        // Para arquivos menores que 4GB, usar método normal
        try {
          if (file.size > 500 * 1024 * 1024) { // 500MB
            console.log('📖 Lendo arquivo grande...', file.name, `${(file.size / 1024 / 1024).toFixed(1)}MB`);
          }
          
          fileBody = await file.arrayBuffer();
          
          if (file.size > 500 * 1024 * 1024) {
            console.log('✅ Arquivo lido com sucesso');
          }
        } catch (arrayBufferError) {
          console.warn('⚠️ Falha no arrayBuffer, tentando FileReader...', arrayBufferError);
          // Fallback: usar FileReader se arrayBuffer falhar
          fileBody = await new Promise<ArrayBuffer>((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = () => resolve(reader.result as ArrayBuffer);
            reader.onerror = () => reject(new Error('Erro ao ler arquivo com FileReader'));
            reader.readAsArrayBuffer(file);
          });
        }
      }

      // Detectar Content-Type correto com base na extensão
      let contentType = file.type;
      if (!contentType || contentType === 'application/octet-stream') {
        const fileName = file.name.toLowerCase();
        if (fileName.endsWith('.csv')) {
          contentType = 'text/csv';
        } else if (fileName.endsWith('.xml')) {
          contentType = 'application/xml';
        } else if (fileName.endsWith('.json')) {
          contentType = 'application/json';
        } else if (fileName.endsWith('.xlsx')) {
          contentType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';
        } else if (fileName.endsWith('.xls')) {
          contentType = 'application/vnd.ms-excel';
        } else {
          contentType = 'application/octet-stream';
        }
      }

      // Preparar metadados
      const metadata: Record<string, string> = {
        'original-filename': file.name,
        'data-type': dataType,
        'file-format': fileFormat,
        'description': description || '',
        'upload-date': new Date().toISOString(),
        'file-size': file.size.toString(),
        'content-type': contentType,
      };

      // Preparar comando de upload - usar ArrayBuffer
      const uploadCommand = new PutObjectCommand({
        Bucket: S3_CONFIG.bucketName,
        Key: key,
        Body: fileBody,
        ContentType: contentType,
        Metadata: metadata,
      });

      // Fazer upload para S3
      console.log(`🚀 Iniciando upload para S3:`, {
        fileName: file.name,
        size: `${(file.size / 1024 / 1024).toFixed(1)}MB`,
        contentType,
        key
      });
      
      const uploadResponse = await s3Client.send(uploadCommand);
      
      console.log(`✅ Upload concluído com sucesso:`, {
        fileName: file.name,
        key,
        etag: uploadResponse.ETag
      });
      
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
          errorMessage = `Erro ao ler o arquivo (${(file.size / 1024 / 1024 / 1024).toFixed(1)}GB). Arquivos muito grandes podem exceder a capacidade do navegador. Limite máximo: 5GB.`;
        } else if (error.message.includes('Arquivo muito grande para processar')) {
          errorMessage = error.message;
        } else if (error.message.includes('readableStream.getReader is not a function')) {
          errorMessage = 'Erro de compatibilidade de stream. Para arquivos muito grandes, tente usar arquivos menores que 5GB.';
        } else if (error.message.includes('NetworkingError') || error.message.includes('Network')) {
          errorMessage = 'Erro de conexão. Verifique sua internet e tente novamente. Para arquivos grandes, uma conexão estável é essencial.';
        } else if (error.message.includes('TimeoutError') || error.message.includes('timeout')) {
          errorMessage = 'Timeout no upload. Arquivo muito grande ou conexão lenta. Tente novamente ou use uma conexão mais rápida.';
        } else if (error.message.includes('AccessDenied')) {
          errorMessage = 'Erro de permissão. Verifique as credenciais AWS.';
        } else if (error.message.includes('RequestTimeout')) {
          errorMessage = 'Timeout na requisição. Para arquivos grandes (até 5GB), mantenha uma conexão estável.';
        } else if (error.message.includes('EntityTooLarge')) {
          errorMessage = 'Arquivo excede o limite máximo do S3 (5GB). Considere dividir o arquivo.';
        } else if (error.message.includes('out of memory') || error.message.includes('Maximum call stack')) {
          errorMessage = `Arquivo muito grande (${(file.size / 1024 / 1024 / 1024).toFixed(1)}GB) para o navegador processar. Limite máximo: 5GB.`;
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