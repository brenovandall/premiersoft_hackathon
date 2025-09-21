import React from 'react';
import { AlertTriangle, CheckCircle2, XCircle } from 'lucide-react';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { useHealthCheck } from '@/hooks/useGetData';
import { 
  EstadosCard, 
  MunicipiosCard, 
  PacientesCard, 
  MedicosCard, 
  HospitaisCard 
} from '@/components/data';

function ApiStatusCard() {
  const { isHealthy, loading, error, checkHealth } = useHealthCheck();

  if (loading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Status da API</CardTitle>
        </CardHeader>
        <CardContent>
          <Skeleton className="h-12 w-full" />
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <CardTitle>Status da API</CardTitle>
        <Button onClick={checkHealth} variant="outline" size="sm">
          Verificar
        </Button>
      </CardHeader>
      <CardContent>
        {error ? (
          <Alert>
            <XCircle className="h-4 w-4" />
            <AlertDescription className="flex items-center justify-between">
              <span>{error}</span>
              <Badge variant="destructive" className="ml-2">
                Offline
              </Badge>
            </AlertDescription>
          </Alert>
        ) : isHealthy ? (
          <Alert>
            <CheckCircle2 className="h-4 w-4" />
            <AlertDescription className="flex items-center justify-between">
              <span>API funcionando normalmente</span>
              <Badge className="bg-green-100 text-green-800 ml-2">
                Online
              </Badge>
            </AlertDescription>
          </Alert>
        ) : (
          <Alert>
            <AlertTriangle className="h-4 w-4" />
            <AlertDescription className="flex items-center justify-between">
              <span>Status da API desconhecido</span>
              <Badge variant="secondary" className="ml-2">
                Desconhecido
              </Badge>
            </AlertDescription>
          </Alert>
        )}
      </CardContent>
    </Card>
  );
}

export function DataDashboard() {
  return (
    <div className="container mx-auto p-4 space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Dashboard de Dados</h1>
          <p className="text-muted-foreground">
            Visualize e gerencie os dados importados do sistema
          </p>
        </div>
      </div>

      {/* Status da API */}
      <ApiStatusCard />

      {/* Grid de Cards de Dados */}
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
        <EstadosCard />
        <MunicipiosCard />
        <PacientesCard />
        <MedicosCard />
        <HospitaisCard />
      </div>
    </div>
  );
}