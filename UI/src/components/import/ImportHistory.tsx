import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { CheckCircle, AlertTriangle, XCircle, Clock, Eye } from "lucide-react";
import { ImportRecord } from "@/types/import";
import { mockImportHistory } from "@/data/mockData";

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
  return (
    <div className="space-y-4">
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
          {mockImportHistory.map((record) => (
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
          ))}
        </TableBody>
      </Table>
    </div>
  );
};