import React, { useState, useRef } from 'react';
import { Upload, FileSpreadsheet, AlertCircle, CheckCircle } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Badge } from '@/components/ui/badge';
import { useToast } from '@/hooks/use-toast';
import { 
  uploadBackupFile, 
  validateBackupFile,
  DATA_TYPE_LABELS,
  FILE_FORMAT_LABELS,
  type BackupUploadRequest,
  type BackupUploadResponse 
} from '@/services/backupUploadService';

interface UploadBackupProps {
  className?: string;
}

export function UploadBackup({ className }: UploadBackupProps) {
  const [selectedDataType, setSelectedDataType] = useState<string>('');
  const [selectedFileFormat, setSelectedFileFormat] = useState<string>('');
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [isUploading, setIsUploading] = useState(false);
  const [uploadStatus, setUploadStatus] = useState<'idle' | 'success' | 'error'>('idle');
  const [statusMessage, setStatusMessage] = useState('');
  const fileInputRef = useRef<HTMLInputElement>(null);
  const { toast } = useToast();

  const dataTypes = Object.entries(DATA_TYPE_LABELS).map(([value, label]) => ({
    value,
    label
  }));

  const fileFormats = Object.entries(FILE_FORMAT_LABELS).map(([value, label]) => ({
    value,
    label
  }));

  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      setSelectedFile(file);
      setUploadStatus('idle');
      setStatusMessage('');
    }
  };

  const handleUploadClick = () => {
    fileInputRef.current?.click();
  };

  const handleBackupUpload = async () => {
    if (!selectedFile || !selectedDataType || !selectedFileFormat) {
      toast({
        title: "Erro de validação",
        description: "Por favor, selecione o arquivo, tipo de dados e formato do arquivo.",
        variant: "destructive",
      });
      return;
    }

    // Validar arquivo
    const validation = validateBackupFile(selectedFile);
    if (!validation.valid) {
      toast({
        title: "Arquivo inválido",
        description: validation.error,
        variant: "destructive",
      });
      return;
    }

    setIsUploading(true);
    setUploadStatus('idle');
    setStatusMessage('');

    try {
      const backupRequest: BackupUploadRequest = {
        dataType: parseInt(selectedDataType),
        fileFormat: parseInt(selectedFileFormat),
        fileName: selectedFile.name,
        fileContent: '' // Será preenchido pela função uploadBackupFile
      };

      const result = await uploadBackupFile(selectedFile, backupRequest);

      if (result.success) {
        setUploadStatus('success');
        setStatusMessage(`Importação concluída! ${result.successfulRecords} registros processados com sucesso.`);
        toast({
          title: "Importação concluída",
          description: `${result.successfulRecords} registros importados diretamente no banco de dados.`,
        });
        
        // Reset form
        setSelectedFile(null);
        setSelectedDataType('');
        setSelectedFileFormat('');
        if (fileInputRef.current) {
          fileInputRef.current.value = '';
        }
      } else {
        throw new Error(result.message);
      }
    } catch (error) {
      console.error('Erro na importação direta:', error);
      setUploadStatus('error');
      setStatusMessage(error instanceof Error ? error.message : 'Erro na importação');
      toast({
        title: "Erro na importação",
        description: "Falha ao importar arquivo diretamente no banco. Tente novamente.",
        variant: "destructive",
      });
    } finally {
      setIsUploading(false);
    }
  };

  const canUpload = selectedFile && selectedDataType && selectedFileFormat && !isUploading;

  return (
    <Card className={className}>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <FileSpreadsheet className="h-5 w-5" />
          Importação Direta
        </CardTitle>
        <p className="text-sm text-muted-foreground">
          Importação direta no banco de dados (sem tabela import)
        </p>
      </CardHeader>
      <CardContent className="space-y-4">
        {/* File Selection */}
        <div className="space-y-2">
          <label className="text-sm font-medium">Arquivo</label>
          <div className="flex items-center gap-2">
            <Button
              variant="outline"
              onClick={handleUploadClick}
              className="flex items-center gap-2"
              disabled={isUploading}
            >
              <Upload className="h-4 w-4" />
              Selecionar Arquivo
            </Button>
            {selectedFile && (
              <Badge variant="secondary" className="flex items-center gap-1">
                {selectedFile.name}
                <span className="text-xs">({(selectedFile.size / 1024).toFixed(1)}KB)</span>
              </Badge>
            )}
          </div>
          <input
            ref={fileInputRef}
            type="file"
            onChange={handleFileSelect}
            className="hidden"
            accept=".csv,.xlsx,.xls"
          />
        </div>

        {/* Data Type Selection */}
        <div className="space-y-2">
          <label className="text-sm font-medium">Tipo de Dados</label>
          <Select value={selectedDataType} onValueChange={setSelectedDataType} disabled={isUploading}>
            <SelectTrigger>
              <SelectValue placeholder="Selecione o tipo de dados" />
            </SelectTrigger>
            <SelectContent>
              {dataTypes.map((type) => (
                <SelectItem key={type.value} value={type.value}>
                  {type.label}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        {/* File Format Selection */}
        <div className="space-y-2">
          <label className="text-sm font-medium">Formato do Arquivo</label>
          <Select value={selectedFileFormat} onValueChange={setSelectedFileFormat} disabled={isUploading}>
            <SelectTrigger>
              <SelectValue placeholder="Selecione o formato" />
            </SelectTrigger>
            <SelectContent>
              {fileFormats.map((format) => (
                <SelectItem key={format.value} value={format.value}>
                  {format.label}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        {/* Upload Status */}
        {uploadStatus !== 'idle' && (
          <Alert className={uploadStatus === 'success' ? 'border-green-200 bg-green-50' : 'border-red-200 bg-red-50'}>
            {uploadStatus === 'success' ? (
              <CheckCircle className="h-4 w-4 text-green-600" />
            ) : (
              <AlertCircle className="h-4 w-4 text-red-600" />
            )}
            <AlertDescription className={uploadStatus === 'success' ? 'text-green-800' : 'text-red-800'}>
              {statusMessage}
            </AlertDescription>
          </Alert>
        )}

        {/* Upload Button */}
        <Button
          onClick={handleBackupUpload}
          disabled={!canUpload}
          className="w-full"
          size="lg"
        >
          {isUploading ? (
            <>
              <div className="mr-2 h-4 w-4 animate-spin rounded-full border-2 border-background border-t-foreground" />
              Enviando Backup...
            </>
          ) : (
            <>
              <Upload className="mr-2 h-4 w-4" />
              Importar Direto
            </>
          )}
        </Button>

        <div className="text-xs text-muted-foreground">
          <p>• Importação direta no banco de dados</p>
          <p>• Não passa pela tabela Import - processamento imediato</p>
          <p>• Suporta apenas Estados e Municípios no momento</p>
          <p>• Formatos suportados: CSV e Excel</p>
        </div>
      </CardContent>
    </Card>
  );
}