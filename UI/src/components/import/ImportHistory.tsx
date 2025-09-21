import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { CheckCircle, AlertTriangle, XCircle, Clock, Eye, RefreshCw } from "lucide-react";
import { ImportRecord } from "@/types/import";
import { getImportRecords } from "@/services/backendService";
import { useEffect, useState } from "react";

const getStatusIcon = (status: ImportRecord["status"]) => {
  switch (status) {
    case "processing":
      return <Clock className="h-4 w-4" />;
    case "success":
      return <CheckCircle className="h-4 w-4" />;
    case "warning":
      return <AlertTriangle className="h-4 w-4" />;
    case "error":
      return <XCircle className="h-4 w-4" />;
    default:
      return <Clock className="h-4 w-4" />;
  }
};

const getStatusVariant = (status: ImportRecord["status"]) => {
  switch (status) {
    case "processing":
      return "secondary";
    case "success":
      return "default";
    case "warning":
      return "outline";
    case "error":
      return "destructive";
    default:
      return "secondary";
  }
};

const getDataTypeLabel = (dataType: ImportRecord["dataType"]) => {
  switch (dataType) {
    case "hospitals":
      return "Hospitais";
    case "doctors":
      return "Médicos";
    case "patients":
      return "Pacientes";
    case "locations":
      return "Estados/Municípios";
    case "cid10":
      return "CID-10";
    default:
      return dataType;
  }
};

const getStatusLabel = (status: ImportRecord["status"]) => {
  switch (status) {
    case "processing":
      return "Processando...";
    case "success":
      return "Concluído";
    case "warning":
      return "Concluído com Avisos";
    case "error":
      return "Falha";
    default:
      return status;
  }
};

interface ImportHistoryProps {
  onViewDetails: (record: ImportRecord) => void;
}

export const ImportHistory = ({ onViewDetails }: ImportHistoryProps) => {
  const [importRecords, setImportRecords] = useState<ImportRecord[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchImportHistory = async () => {
    try {
      setLoading(true);
      setError(null);
      
      const result = await getImportRecords();
      
      if (result.success && result.data) {
        setImportRecords(result.data);
      } else {
        setError(result.error || 'Erro ao carregar histórico');
      }
    } catch (err) {
      setError('Erro ao carregar histórico de importações');
      console.error('Erro ao buscar histórico:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchImportHistory();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center p-8">
        <RefreshCw className="h-6 w-6 animate-spin" />
        <span className="ml-2">Carregando histórico...</span>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex flex-col items-center justify-center p-8 space-y-4">
        <div className="text-red-600">❌ {error}</div>
        <Button onClick={fetchImportHistory} variant="outline">
          <RefreshCw className="h-4 w-4 mr-2" />
          Tentar Novamente
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <div className="text-sm text-muted-foreground">
          {importRecords.length} importação(ões) encontrada(s)
        </div>
        <Button onClick={fetchImportHistory} variant="outline" size="sm">
          <RefreshCw className="h-4 w-4 mr-2" />
          Atualizar
        </Button>
      </div>
      
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Arquivo</TableHead>
            <TableHead>Tipo de Dado</TableHead>
            <TableHead>Data de Envio</TableHead>
            <TableHead>Status</TableHead>
            <TableHead>Resumo</TableHead>
            <TableHead>Ações</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {importRecords.length === 0 ? (
            <TableRow>
              <TableCell colSpan={6} className="text-center py-8 text-muted-foreground">
                Nenhuma importação encontrada
              </TableCell>
            </TableRow>
          ) : (
            importRecords.map((record) => (
            <TableRow key={record.id}>
              <TableCell className="font-medium">
                {record.fileName}
              </TableCell>
              <TableCell>
                {getDataTypeLabel(record.dataType)}
              </TableCell>
              <TableCell>
                {record.uploadDate.toLocaleDateString("pt-BR")} {record.uploadDate.toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" })}
              </TableCell>
              <TableCell>
                <Badge variant={getStatusVariant(record.status)} className="flex items-center gap-1 w-fit">
                  {getStatusIcon(record.status)}
                  {getStatusLabel(record.status)}
                </Badge>
              </TableCell>
              <TableCell>
                <span className="text-sm">{record.summary}</span>
              </TableCell>
              <TableCell>
                {(record.status === "warning" || record.status === "error") && (
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => onViewDetails(record)}
                    className="flex items-center gap-1"
                  >
                    <Eye className="h-3 w-3" />
                    Ver Detalhes
                  </Button>
                )}
              </TableCell>
            </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </div>
  );
};