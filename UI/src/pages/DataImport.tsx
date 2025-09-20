import { useState } from "react";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/components/ui/tabs";
import ImportForm from "@/components/import/ImportForm";
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
          <CardContent className="p-6">
            <Tabs defaultValue="import" className="w-full">
              <TabsList className="grid w-full grid-cols-2">
                <TabsTrigger value="import">Importação</TabsTrigger>
                <TabsTrigger value="history">Histórico</TabsTrigger>
              </TabsList>
              
              <TabsContent value="import" className="mt-6">
                <div className="space-y-4">
                  <ImportForm />
                </div>
              </TabsContent>
              
              <TabsContent value="history" className="mt-6">
                <div className="space-y-4">
                  <div>
                    <h2 className="text-xl font-semibold">Histórico de Importações</h2>
                    <p className="text-muted-foreground text-sm">
                      Visualize o status e detalhes das importações anteriores
                    </p>
                  </div>
                  <ImportHistory onViewDetails={handleViewDetails} />
                </div>
              </TabsContent>
            </Tabs>
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