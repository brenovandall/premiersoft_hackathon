import React from 'react';
import { AlertCircle, RefreshCw } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { useEstados } from '@/hooks/useGetData';

export function EstadosCard() {
  const { data: estados, loading, error, refresh } = useEstados();

  const getRegiaoBadgeColor = (regiao: string) => {
    const colors: Record<string, string> = {
      'Norte': 'bg-green-100 text-green-800',
      'Nordeste': 'bg-yellow-100 text-yellow-800',
      'Centro-Oeste': 'bg-red-100 text-red-800',
      'Sudeste': 'bg-blue-100 text-blue-800',
      'Sul': 'bg-purple-100 text-purple-800',
    };
    return colors[regiao] || 'bg-gray-100 text-gray-800';
  };

  if (loading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Estados</CardTitle>
          <CardDescription>Carregando dados dos estados...</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-2">
            {[...Array(5)].map((_, i) => (
              <Skeleton key={i} className="h-12 w-full" />
            ))}
          </div>
        </CardContent>
      </Card>
    );
  }

  if (error) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Estados</CardTitle>
          <CardDescription>Dados dos estados brasileiros</CardDescription>
        </CardHeader>
        <CardContent>
          <Alert>
            <AlertCircle className="h-4 w-4" />
            <AlertDescription>{error}</AlertDescription>
          </Alert>
          <Button onClick={refresh} className="mt-4" variant="outline">
            <RefreshCw className="h-4 w-4 mr-2" />
            Tentar novamente
          </Button>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <div>
          <CardTitle>Estados</CardTitle>
          <CardDescription>
            {estados.length} estados encontrados
          </CardDescription>
        </div>
        <Button onClick={refresh} variant="outline" size="sm">
          <RefreshCw className="h-4 w-4" />
        </Button>
      </CardHeader>
      <CardContent>
        <div className="grid gap-2 max-h-96 overflow-y-auto">
          {estados.map((estado) => (
            <div
              key={estado.id}
              className="flex items-center justify-between p-3 border rounded-lg hover:bg-gray-50"
            >
              <div className="flex-1">
                <div className="flex items-center gap-2">
                  <span className="font-medium">{estado.nome}</span>
                  <Badge variant="secondary" className="text-xs">
                    {estado.uf}
                  </Badge>
                </div>
                <p className="text-sm text-gray-500">
                  Código UF: {estado.codigo_uf}
                </p>
              </div>
              <Badge className={getRegiaoBadgeColor(estado.regiao)}>
                {estado.regiao}
              </Badge>
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  );
}