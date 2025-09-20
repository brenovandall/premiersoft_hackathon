import { useState } from "react";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { ImportForm } from "@/components/import/ImportForm";
import { ImportHistory } from "@/components/import/ImportHistory";
import { ErrorDetailsModal } from "@/components/import/ErrorDetailsModal";
import { ImportRecord } from "@/types/import";

const DataImport = () => {
  const [selectedRecord, setSelectedRecord] = useState<ImportRecord | null>(null);
  const [isErrorModalOpen, setIsErrorModalOpen] = useState(false);

  const handleViewDetails = (record: ImportRecord) => {
    setSelectedRecord(record);
    setIsErrorModalOpen(true);
  };

  return (
    <div className="min-h-screen bg-gradient-subtle p-6">
      <div className="mx-auto max-w-7xl space-y-6">
        <div>
          <h1 className="text-3xl font-bold tracking-tight text-foreground">
            Importação de Dados
          </h1>
          <p className="text-muted-foreground">
            Importe dados de hospitais, médicos, pacientes e outras informações do sistema de saúde
          </p>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>Upload de Arquivo</CardTitle>
          </CardHeader>
          <CardContent>
            <ImportForm />
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Histórico de Importações</CardTitle>
          </CardHeader>
          <CardContent>
            <ImportHistory onViewDetails={handleViewDetails} />
          </CardContent>
        </Card>

        <ErrorDetailsModal
          record={selectedRecord}
          open={isErrorModalOpen}
          onOpenChange={setIsErrorModalOpen}
        />
      </div>
    </div>
  );
};

export default DataImport;