import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Badge } from "@/components/ui/badge";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { CheckCircle, AlertTriangle, XCircle } from "lucide-react";
import { ImportRecord } from "@/types/import";

interface ErrorDetailsModalProps {
  record: ImportRecord | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export const ErrorDetailsModal = ({ record, open, onOpenChange }: ErrorDetailsModalProps) => {
  if (!record) return null;

  const getStatusIcon = (status: ImportRecord["status"]) => {
    switch (status) {
      case "success":
        return <CheckCircle className="h-5 w-5 text-success" />;
      case "warning":
        return <AlertTriangle className="h-5 w-5 text-warning" />;
      case "error":
        return <XCircle className="h-5 w-5 text-destructive" />;
      default:
        return null;
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            {getStatusIcon(record.status)}
            Detalhes da Importação: {record.fileName}
          </DialogTitle>
        </DialogHeader>

        <div className="space-y-6">
          {/* Summary Cards */}
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  Total de Registros
                </CardTitle>
              </CardHeader>
              <CardContent className="pt-0">
                <div className="text-2xl font-bold text-foreground">
                  {record.totalRecords || 0}
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  Importados com Sucesso
                </CardTitle>
              </CardHeader>
              <CardContent className="pt-0">
                <div className="text-2xl font-bold text-success">
                  {record.successfulRecords || 0}
                </div>
              </CardContent>
            </Card>

            {record.duplicateRecords && record.duplicateRecords > 0 && (
              <Card>
                <CardHeader className="pb-2">
                  <CardTitle className="text-sm font-medium text-muted-foreground">
                    Duplicados (Ignorados)
                  </CardTitle>
                </CardHeader>
                <CardContent className="pt-0">
                  <div className="text-2xl font-bold text-warning">
                    {record.duplicateRecords}
                  </div>
                </CardContent>
              </Card>
            )}

            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground">
                  Registros com Erro
                </CardTitle>
              </CardHeader>
              <CardContent className="pt-0">
                <div className="text-2xl font-bold text-destructive">
                  {record.errorRecords || 0}
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Error Details */}
          {record.errors && record.errors.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle>Detalhes dos Erros</CardTitle>
              </CardHeader>
              <CardContent>
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Linha</TableHead>
                      <TableHead>Campo</TableHead>
                      <TableHead>Erro</TableHead>
                      <TableHead>Dado Problemático</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {record.errors.map((error, index) => (
                      <TableRow key={index}>
                        <TableCell>
                          <Badge variant="outline">Linha {error.line}</Badge>
                        </TableCell>
                        <TableCell className="font-medium">
                          {error.field}
                        </TableCell>
                        <TableCell className="text-sm">
                          {error.message}
                        </TableCell>
                        <TableCell className="text-sm font-mono bg-muted/50 rounded px-2 py-1">
                          {error.data || "N/A"}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          )}

          {/* File Info */}
          <Card>
            <CardHeader>
              <CardTitle>Informações do Arquivo</CardTitle>
            </CardHeader>
            <CardContent className="space-y-2">
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <span className="font-medium">Nome do Arquivo:</span> {record.fileName}
                </div>
                <div>
                  <span className="font-medium">Tipo de Dado:</span> {record.dataType}
                </div>
                <div>
                  <span className="font-medium">Formato:</span> {record.fileFormat.toUpperCase()}
                </div>
                <div>
                  <span className="font-medium">Data de Upload:</span> {record.uploadDate.toLocaleString("pt-BR")}
                </div>
              </div>
              {record.description && (
                <div className="text-sm">
                  <span className="font-medium">Descrição:</span> {record.description}
                </div>
              )}
            </CardContent>
          </Card>
        </div>
      </DialogContent>
    </Dialog>
  );
};