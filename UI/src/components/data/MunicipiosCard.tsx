import React, { useState } from 'react';
import { AlertCircle, RefreshCw, MapPin } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useMunicipios, useEstados } from '@/hooks/useGetData';

export function MunicipiosCard() {
  const [selectedEstado, setSelectedEstado] = useState<string>('');
  const { data: estados } = useEstados();
  const { data: municipios, loading, error, refresh } = useMunicipios(selectedEstado || undefined);

  const formatPopulacao = (populacao: number) => {
    return new Intl.NumberFormat('pt-BR').format(populacao);
  };

  if (loading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Municípios</CardTitle>
          <CardDescription>Carregando dados dos municípios...</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-2">
            <Skeleton className="h-10 w-full" />
            {[...Array(5)].map((_, i) => (
              <Skeleton key={i} className="h-16 w-full" />
            ))}
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <div>
          <CardTitle>Municípios</CardTitle>
          <CardDescription>
            {municipios.length} municípios encontrados
            {selectedEstado && ` no estado selecionado`}
          </CardDescription>
        </div>
        <Button onClick={refresh} variant="outline" size="sm">
          <RefreshCw className="h-4 w-4" />
        </Button>
      </CardHeader>
      <CardContent>
        <div className="mb-4">
          <Select value={selectedEstado} onValueChange={setSelectedEstado}>
            <SelectTrigger>
              <SelectValue placeholder="Filtrar por estado..." />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="">Todos os estados</SelectItem>
              {estados.map((estado) => (
                <SelectItem key={estado.codigo_uf} value={estado.codigo_uf.toString()}>
                  {estado.nome} ({estado.uf})
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        {error ? (
          <Alert>
            <AlertCircle className="h-4 w-4" />
            <AlertDescription>{error}</AlertDescription>
          </Alert>
        ) : (
          <div className="grid gap-2 max-h-96 overflow-y-auto">
            {municipios.map((municipio) => (
              <div
                key={municipio.id}
                className="flex items-center justify-between p-3 border rounded-lg hover:bg-gray-50"
              >
                <div className="flex-1">
                  <div className="flex items-center gap-2">
                    <MapPin className="h-4 w-4 text-gray-400" />
                    <span className="font-medium">{municipio.nome}</span>
                    {municipio.capital && (
                      <Badge variant="default" className="text-xs bg-yellow-100 text-yellow-800">
                        Capital
                      </Badge>
                    )}
                  </div>
                  <div className="text-sm text-gray-500 mt-1">
                    <p>IBGE: {municipio.codigo_ibge} | DDD: {municipio.ddd}</p>
                    <p>População: {formatPopulacao(municipio.populacao)}</p>
                  </div>
                </div>
                <div className="text-right text-xs text-gray-400">
                  <p>Lat: {municipio.latitude.toFixed(2)}</p>
                  <p>Lng: {municipio.longitude.toFixed(2)}</p>
                </div>
              </div>
            ))}
          </div>
        )}
      </CardContent>
    </Card>
  );
}