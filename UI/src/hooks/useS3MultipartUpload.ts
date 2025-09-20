import { useState } from 'react';
import {
    CreateMultipartUploadCommand,
    UploadPartCommand,
    CompleteMultipartUploadCommand,
    AbortMultipartUploadCommand,
    PutObjectCommand
} from '@aws-sdk/client-s3';
import { getSignedUrl } from '@aws-sdk/s3-request-presigner';
import { s3Client, S3_CONFIG, generateS3Key, getS3PublicUrl, validateS3Config } from '@/lib/s3Client';

interface UploadProgress {
    loaded: number;
    total: number;
    percentage: number;
    currentPart?: number;
    totalParts?: number;
    phase?: 'reading' | 'uploading' | 'completing';
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

export const useS3MultipartUpload = () => {
    const [isUploading, setIsUploading] = useState(false);
    const [progress, setProgress] = useState<UploadProgress | null>(null);
    const [error, setError] = useState<string | null>(null);

    // Função para calcular tamanho ideal do chunk baseado no arquivo
    const calculateOptimalChunkSize = (fileSize: number): number => {
        if (fileSize < 500 * 1024 * 1024) { // < 500MB
            return 25 * 1024 * 1024; // 25MB chunks = ~20 partes
        } else if (fileSize < 2 * 1024 * 1024 * 1024) { // < 2GB
            return 50 * 1024 * 1024; // 50MB chunks = ~40 partes para 2GB
        } else if (fileSize < 5 * 1024 * 1024 * 1024) { // < 5GB
            return 100 * 1024 * 1024; // 100MB chunks = ~26 partes para 2.6GB
        } else {
            return 200 * 1024 * 1024; // 200MB chunks para arquivos muito grandes
        }
    };

    // Função para calcular concorrência ideal baseada no arquivo
    const calculateOptimalConcurrency = (fileSize: number): number => {
        if (fileSize < 500 * 1024 * 1024) { // < 500MB
            return 3; // 3 uploads simultâneos
        } else if (fileSize < 2 * 1024 * 1024 * 1024) { // < 2GB
            return 5; // 5 uploads simultâneos
        } else {
            return 8; // 8 uploads simultâneos para arquivos muito grandes (2.6GB)
        }
    };

    // Constantes para multipart upload
    const MIN_MULTIPART_SIZE = 100 * 1024 * 1024; // 100MB - usar multipart para arquivos maiores

    const uploadFile = async (
        file: File,
        dataType: string,
        fileFormat: string,
        description?: string
    ): Promise<UploadResult> => {
        setIsUploading(true);
        setError(null);
        setProgress({
            loaded: 0,
            total: file.size,
            percentage: 0,
            phase: 'reading'
        });

        try {
            // Validar configuração S3
            const configValidation = validateS3Config();
            if (!configValidation.isValid) {
                throw new Error(`Configuração S3 incompleta. Variáveis faltando: ${configValidation.missingVars.join(', ')}`);
            }

            // Validações básicas
            if (!file || file.size === 0) {
                throw new Error('Arquivo vazio ou inválido');
            }

            if (file.size > S3_CONFIG.maxFileSize) {
                throw new Error(`Arquivo muito grande. Máximo permitido: ${(S3_CONFIG.maxFileSize / 1024 / 1024 / 1024).toFixed(1)}GB`);
            }

            // Calcular configurações otimizadas para este arquivo
            const chunkSize = calculateOptimalChunkSize(file.size);
            const maxConcurrent = calculateOptimalConcurrency(file.size);
            const estimatedParts = Math.ceil(file.size / chunkSize);

            // Gerar chave e detectar content type
            const key = generateS3Key(dataType, file.name);
            const contentType = detectContentType(file);

            console.log(`🚀 Iniciando upload: ${file.name} (${(file.size / 1024 / 1024).toFixed(1)}MB)`);
            console.log(`⚡ Configuração otimizada: ${(chunkSize / 1024 / 1024)}MB por parte, ${maxConcurrent} uploads simultâneos, ~${estimatedParts} partes`);

            // Decidir entre upload simples ou multipart
            if (file.size >= MIN_MULTIPART_SIZE) {
                return await uploadMultipart(file, key, contentType, dataType, fileFormat, description, chunkSize, maxConcurrent);
            } else {
                return await uploadSimple(file, key, contentType, dataType, fileFormat, description);
            }

        } catch (error) {
            console.error('❌ Erro no upload:', error);

            const errorMessage = error instanceof Error ? error.message : 'Erro desconhecido no upload';
            setError(errorMessage);

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

    const uploadSimple = async (
        file: File,
        key: string,
        contentType: string,
        dataType: string,
        fileFormat: string,
        description?: string
    ): Promise<UploadResult> => {
        console.log('📤 Upload simples');

        setProgress(prev => prev ? { ...prev, phase: 'uploading' } : null);

        // Converter File para Uint8Array para compatibilidade com browser
        const arrayBuffer = await file.arrayBuffer();
        const uint8Array = new Uint8Array(arrayBuffer);

        // Preparar metadados
        const metadata = createMetadata(file, dataType, fileFormat, description);

        // Preparar comando de upload
        const uploadCommand = new PutObjectCommand({
            Bucket: S3_CONFIG.bucketName,
            Key: key,
            Body: uint8Array, // Usar Uint8Array ao invés de File
            ContentType: contentType,
            Metadata: metadata,
        });

        // Executar upload
        const uploadResponse = await s3Client.send(uploadCommand);

        setProgress({
            loaded: file.size,
            total: file.size,
            percentage: 100,
            phase: 'completing'
        });

        console.log('✅ Upload simples concluído');

        return {
            success: true,
            message: 'Arquivo enviado com sucesso',
            data: {
                key,
                fileName: file.name,
                size: file.size,
                url: getS3PublicUrl(key),
                bucket: S3_CONFIG.bucketName,
                etag: uploadResponse.ETag,
            },
        };
    };

    const uploadMultipart = async (
        file: File,
        key: string,
        contentType: string,
        dataType: string,
        fileFormat: string,
        description?: string,
        chunkSize?: number,
        maxConcurrent?: number
    ): Promise<UploadResult> => {
        console.log('🔄 Upload multipart');

        // Usar configurações otimizadas ou valores padrão
        const actualChunkSize = chunkSize || calculateOptimalChunkSize(file.size);
        const actualMaxConcurrent = maxConcurrent || calculateOptimalConcurrency(file.size);

        let uploadId: string | undefined;

        try {
            // 1. Iniciar upload multipart
            const createCommand = new CreateMultipartUploadCommand({
                Bucket: S3_CONFIG.bucketName,
                Key: key,
                ContentType: contentType,
                Metadata: createMetadata(file, dataType, fileFormat, description),
            });

            const createResponse = await s3Client.send(createCommand);
            uploadId = createResponse.UploadId!;

            console.log(`📋 Upload multipart iniciado: ${uploadId}`);

            // 2. Calcular número de partes com novo chunk size
            const totalParts = Math.ceil(file.size / actualChunkSize);

            setProgress(prev => prev ? {
                ...prev,
                phase: 'uploading',
                totalParts,
                currentPart: 0
            } : null);

            console.log(`📦 Dividindo em ${totalParts} partes de ${(actualChunkSize / 1024 / 1024).toFixed(1)}MB`);
            console.log(`🚀 Usando ${actualMaxConcurrent} uploads simultâneos para máxima velocidade`);

            // 3. Upload das partes
            const parts = await uploadParts(file, key, uploadId, totalParts, actualChunkSize, actualMaxConcurrent);

            // 4. Completar upload multipart
            setProgress(prev => prev ? { ...prev, phase: 'completing' } : null);

            const completeCommand = new CompleteMultipartUploadCommand({
                Bucket: S3_CONFIG.bucketName,
                Key: key,
                UploadId: uploadId,
                MultipartUpload: {
                    Parts: parts.sort((a, b) => a.PartNumber - b.PartNumber),
                },
            });

            const completeResponse = await s3Client.send(completeCommand);

            console.log('✅ Upload multipart concluído');

            setProgress({
                loaded: file.size,
                total: file.size,
                percentage: 100,
                totalParts,
                currentPart: totalParts,
                phase: 'completing'
            });

            return {
                success: true,
                message: 'Arquivo enviado com sucesso (multipart)',
                data: {
                    key,
                    fileName: file.name,
                    size: file.size,
                    url: getS3PublicUrl(key),
                    bucket: S3_CONFIG.bucketName,
                    uploadId,
                    etag: completeResponse.ETag,
                },
            };

        } catch (error) {
            // Abortar upload em caso de erro
            if (uploadId) {
                try {
                    console.log('🚫 Abortando upload multipart...');
                    await s3Client.send(new AbortMultipartUploadCommand({
                        Bucket: S3_CONFIG.bucketName,
                        Key: key,
                        UploadId: uploadId,
                    }));
                } catch (abortError) {
                    console.warn('⚠️ Erro ao abortar upload:', abortError);
                }
            }
            throw error;
        }
    };

    const uploadParts = async (
        file: File,
        key: string,
        uploadId: string,
        totalParts: number,
        chunkSize?: number,
        maxConcurrent?: number
    ): Promise<MultipartPart[]> => {
        const parts: MultipartPart[] = [];
        const uploadPromises: Promise<MultipartPart>[] = [];
        
        const actualChunkSize = chunkSize || calculateOptimalChunkSize(file.size);
        const actualMaxConcurrent = maxConcurrent || calculateOptimalConcurrency(file.size);

        // Função para upload de uma parte com retry
        const uploadPart = async (partNumber: number, retries = 3): Promise<MultipartPart> => {
            const start = (partNumber - 1) * actualChunkSize;
            const end = Math.min(start + actualChunkSize, file.size);
            const chunk = file.slice(start, end);

            console.log(`📤 Enviando parte ${partNumber}/${totalParts} (${(chunk.size / 1024 / 1024).toFixed(1)}MB)`);

            for (let attempt = 1; attempt <= retries; attempt++) {
                try {
                    // Converter Blob para Uint8Array para compatibilidade com browser
                    const arrayBuffer = await chunk.arrayBuffer();
                    const uint8Array = new Uint8Array(arrayBuffer);

                    // Criar comando para upload da parte
                    const uploadCommand = new UploadPartCommand({
                        Bucket: S3_CONFIG.bucketName,
                        Key: key,
                        PartNumber: partNumber,
                        UploadId: uploadId,
                        Body: uint8Array, // Usar Uint8Array
                    });

                    const response = await s3Client.send(uploadCommand);

                    // Atualizar progresso
                    const completedParts = parts.length + 1;
                    const loaded = completedParts * actualChunkSize;
                    const percentage = Math.min((loaded / file.size) * 100, 95);

                    setProgress(prev => prev ? {
                        ...prev,
                        loaded: Math.min(loaded, file.size),
                        percentage,
                        currentPart: completedParts,
                    } : null);

                    console.log(`✅ Parte ${partNumber} enviada: ${response.ETag}`);

                    return {
                        PartNumber: partNumber,
                        ETag: response.ETag!,
                    };
                } catch (error) {
                    console.warn(`⚠️ Tentativa ${attempt}/${retries} falhou para parte ${partNumber}:`, error);

                    if (attempt === retries) {
                        throw error; // Re-throw na última tentativa
                    }

                    // Aguardar antes de tentar novamente (backoff exponencial)
                    await new Promise(resolve => setTimeout(resolve, Math.pow(2, attempt) * 1000));
                }
            }

            throw new Error(`Falha ao enviar parte ${partNumber} após ${retries} tentativas`);
        };

        // Upload das partes com controle de concorrência otimizado
        console.log(`🔧 Usando ${actualMaxConcurrent} uploads simultâneos para ${totalParts} partes`);

        for (let partNumber = 1; partNumber <= totalParts; partNumber++) {
            uploadPromises.push(uploadPart(partNumber));

            // Controlar concorrência
            if (uploadPromises.length >= actualMaxConcurrent || partNumber === totalParts) {
                try {
                    const completedParts = await Promise.all(uploadPromises);
                    parts.push(...completedParts);
                    uploadPromises.length = 0; // Limpar array
                    
                    // Log de progresso
                    console.log(`📊 Progresso: ${parts.length}/${totalParts} partes concluídas (${((parts.length / totalParts) * 100).toFixed(1)}%)`);
                } catch (error) {
                    console.error('❌ Erro no lote de uploads:', error);
                    throw error;
                }
            }
        }        return parts;
    };

    const detectContentType = (file: File): string => {
        if (file.type && file.type !== 'application/octet-stream') {
            return file.type;
        }

        const fileName = file.name.toLowerCase();
        if (fileName.endsWith('.csv')) return 'text/csv';
        if (fileName.endsWith('.xml')) return 'application/xml';
        if (fileName.endsWith('.xlsx')) return 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';
        if (fileName.endsWith('.xls')) return 'application/vnd.ms-excel';

        return 'application/octet-stream';
    };

    const createMetadata = (
        file: File,
        dataType: string,
        fileFormat: string,
        description?: string
    ): Record<string, string> => {
        return {
            'original-filename': file.name,
            'data-type': dataType,
            'file-format': fileFormat,
            'description': description || '',
            'upload-date': new Date().toISOString(),
            'file-size': file.size.toString(),
            'content-type': detectContentType(file),
        };
    };

    const reset = () => {
        setIsUploading(false);
        setProgress(null);
        setError(null);
    };

    return {
        uploadFile,
        isUploading,
        progress,
        error,
        reset,
    };
};