import React from 'react';
import { AlertCircle, RefreshCw, Building2, Bed } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { useHospitais } from '@/hooks/useGetData';

export function HospitaisCard() {
  const { data: hospitais, loading, error, refresh } = useHospitais();

  const formatLeitos = (leitos: number) => {
    return new Intl.NumberFormat('pt-BR').format(leitos);
  };

  const getLeitosBadgeColor = (leitos: number) => {
    if (leitos >= 1000) return 'bg-red-100 text-red-800';
    if (leitos >= 500) return 'bg-orange-100 text-orange-800';
    if (leitos >= 200) return 'bg-yellow-100 text-yellow-800';
    if (leitos >= 50) return 'bg-green-100 text-green-800';
    return 'bg-blue-100 text-blue-800';
  };

  const getLeitosSize = (leitos: number) => {
    if (leitos >= 1000) return 'Grande Porte';
    if (leitos >= 500) return 'Médio Porte';
    if (leitos >= 200) return 'Pequeno Porte';
    if (leitos >= 50) return 'Micro Porte';
    return 'Muito Pequeno';
  };

  if (loading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Hospitais</CardTitle>
          <CardDescription>Carregando dados dos hospitais...</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-2">
            {[...Array(5)].map((_, i) => (
              <Skeleton key={i} className="h-16 w-full" />
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
          <CardTitle>Hospitais</CardTitle>
          <CardDescription>Dados dos hospitais cadastrados</CardDescription>
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

  const totalLeitos = hospitais.reduce((sum, hospital) => sum + hospital.leitos_totais, 0);
  const mediaLeitos = Math.round(totalLeitos / hospitais.length);

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <div>
          <CardTitle className="flex items-center gap-2">
            <Building2 className="h-5 w-5" />
            Hospitais
          </CardTitle>
          <CardDescription>
            {hospitais.length} hospitais cadastrados
          </CardDescription>
        </div>
        <Button onClick={refresh} variant="outline" size="sm">
          <RefreshCw className="h-4 w-4" />
        </Button>
      </CardHeader>
      <CardContent>
        {/* Estatísticas rápidas */}
        <div className="grid grid-cols-2 gap-4 mb-4">
          <div className="flex items-center gap-2 p-2 bg-blue-50 rounded-lg">
            <Bed className="h-4 w-4 text-blue-600" />
            <div className="text-sm">
              <p className="font-medium text-blue-800">Total de Leitos</p>
              <p className="text-blue-600">{formatLeitos(totalLeitos)}</p>
            </div>
          </div>
          <div className="flex items-center gap-2 p-2 bg-green-50 rounded-lg">
            <Building2 className="h-4 w-4 text-green-600" />
            <div className="text-sm">
              <p className="font-medium text-green-800">Média por Hospital</p>
              <p className="text-green-600">{formatLeitos(mediaLeitos)}</p>
            </div>
          </div>
        </div>

        <div className="grid gap-2 max-h-96 overflow-y-auto">
          {hospitais.map((hospital) => (
            <div
              key={hospital.id}
              className="flex items-center justify-between p-3 border rounded-lg hover:bg-gray-50"
            >
              <div className="flex-1">
                <div className="flex items-center gap-2">
                  <span className="text-lg">🏥</span>
                  <div>
                    <p className="font-medium">{hospital.nome}</p>
                    <p className="text-sm text-gray-500">
                      {hospital.bairro}, {hospital.cidade}
                    </p>
                  </div>
                </div>
                <div className="mt-2 flex items-center gap-2">
                  <Badge 
                    className={getLeitosBadgeColor(hospital.leitos_totais)}
                  >
                    {getLeitosSize(hospital.leitos_totais)}
                  </Badge>
                  <span className="text-xs text-gray-500">
                    Código: {hospital.codigo}
                  </span>
                </div>
              </div>
              <div className="text-right">
                <div className="flex items-center gap-1 text-sm font-medium">
                  <Bed className="h-4 w-4 text-gray-500" />
                  <span>{formatLeitos(hospital.leitos_totais)}</span>
                </div>
                <p className="text-xs text-gray-500">leitos</p>
              </div>
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  );
}