import React from 'react';
import { AlertCircle, RefreshCw, Stethoscope, MapPin } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { useMedicos } from '@/hooks/useGetData';

export function MedicosCard() {
  const { data: medicos, loading, error, refresh } = useMedicos();

  const getEspecialidadeBadgeColor = (especialidade: string) => {
    const colors: Record<string, string> = {
      'Cardiologia': 'bg-red-100 text-red-800',
      'Neurologia': 'bg-purple-100 text-purple-800',
      'Pediatria': 'bg-blue-100 text-blue-800',
      'Ortopedia': 'bg-green-100 text-green-800',
      'Dermatologia': 'bg-yellow-100 text-yellow-800',
      'Ginecologia': 'bg-pink-100 text-pink-800',
      'Oftalmologia': 'bg-indigo-100 text-indigo-800',
      'Psiquiatria': 'bg-teal-100 text-teal-800',
    };
    return colors[especialidade] || 'bg-gray-100 text-gray-800';
  };

  if (loading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Médicos</CardTitle>
          <CardDescription>Carregando dados dos médicos...</CardDescription>
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
          <CardTitle>Médicos</CardTitle>
          <CardDescription>Dados dos médicos cadastrados</CardDescription>
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

  // Estatísticas por especialidade
  const especialidadesCount = medicos.reduce((acc, medico) => {
    acc[medico.especialidade] = (acc[medico.especialidade] || 0) + 1;
    return acc;
  }, {} as Record<string, number>);

  const topEspecialidades = Object.entries(especialidadesCount)
    .sort(([,a], [,b]) => b - a)
    .slice(0, 3);

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <div>
          <CardTitle className="flex items-center gap-2">
            <Stethoscope className="h-5 w-5" />
            Médicos
          </CardTitle>
          <CardDescription>
            {medicos.length} médicos cadastrados
          </CardDescription>
        </div>
        <Button onClick={refresh} variant="outline" size="sm">
          <RefreshCw className="h-4 w-4" />
        </Button>
      </CardHeader>
      <CardContent>
        {/* Estatísticas por especialidade */}
        {topEspecialidades.length > 0 && (
          <div className="mb-4">
            <h4 className="text-sm font-medium mb-2">Top Especialidades</h4>
            <div className="flex flex-wrap gap-2">
              {topEspecialidades.map(([especialidade, count]) => (
                <Badge 
                  key={especialidade} 
                  className={getEspecialidadeBadgeColor(especialidade)}
                >
                  {especialidade} ({count})
                </Badge>
              ))}
            </div>
          </div>
        )}

        <div className="grid gap-2 max-h-96 overflow-y-auto">
          {medicos.map((medico) => (
            <div
              key={medico.id}
              className="flex items-center justify-between p-3 border rounded-lg hover:bg-gray-50"
            >
              <div className="flex-1">
                <div className="flex items-center gap-2">
                  <span className="text-lg">👨‍⚕️</span>
                  <div>
                    <p className="font-medium">{medico.nome_completo}</p>
                    <div className="flex items-center gap-1 text-sm text-gray-500">
                      <MapPin className="h-3 w-3" />
                      <span>{medico.municipio}</span>
                    </div>
                  </div>
                </div>
                <div className="mt-2">
                  <p className="text-xs text-gray-500">Código: {medico.codigo}</p>
                </div>
              </div>
              <div className="text-right">
                <Badge className={getEspecialidadeBadgeColor(medico.especialidade)}>
                  {medico.especialidade}
                </Badge>
              </div>
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  );
}