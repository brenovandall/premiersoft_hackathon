import React from 'react';
import { AlertCircle, RefreshCw, Users, Shield, ShieldCheck } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { usePacientes } from '@/hooks/useGetData';

export function PacientesCard() {
  const { data: pacientes, loading, error, refresh } = usePacientes();

  const getGeneroIcon = (genero: string) => {
    if (genero.toLowerCase() === 'm' || genero.toLowerCase() === 'masculino') {
      return '👨';
    }
    if (genero.toLowerCase() === 'f' || genero.toLowerCase() === 'feminino') {
      return '👩';
    }
    return '👤';
  };

  const getGeneroBadgeColor = (genero: string) => {
    if (genero.toLowerCase() === 'm' || genero.toLowerCase() === 'masculino') {
      return 'bg-blue-100 text-blue-800';
    }
    if (genero.toLowerCase() === 'f' || genero.toLowerCase() === 'feminino') {
      return 'bg-pink-100 text-pink-800';
    }
    return 'bg-gray-100 text-gray-800';
  };

  if (loading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Pacientes</CardTitle>
          <CardDescription>Carregando dados dos pacientes...</CardDescription>
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
          <CardTitle>Pacientes</CardTitle>
          <CardDescription>Dados dos pacientes cadastrados</CardDescription>
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

  const pacientesComConvenio = pacientes.filter(p => p.convenio).length;
  const pacientesSemConvenio = pacientes.length - pacientesComConvenio;

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <div>
          <CardTitle className="flex items-center gap-2">
            <Users className="h-5 w-5" />
            Pacientes
          </CardTitle>
          <CardDescription>
            {pacientes.length} pacientes cadastrados
          </CardDescription>
        </div>
        <Button onClick={refresh} variant="outline" size="sm">
          <RefreshCw className="h-4 w-4" />
        </Button>
      </CardHeader>
      <CardContent>
        {/* Estatísticas rápidas */}
        <div className="grid grid-cols-2 gap-4 mb-4">
          <div className="flex items-center gap-2 p-2 bg-green-50 rounded-lg">
            <ShieldCheck className="h-4 w-4 text-green-600" />
            <div className="text-sm">
              <p className="font-medium text-green-800">Com Convênio</p>
              <p className="text-green-600">{pacientesComConvenio}</p>
            </div>
          </div>
          <div className="flex items-center gap-2 p-2 bg-red-50 rounded-lg">
            <Shield className="h-4 w-4 text-red-600" />
            <div className="text-sm">
              <p className="font-medium text-red-800">Sem Convênio</p>
              <p className="text-red-600">{pacientesSemConvenio}</p>
            </div>
          </div>
        </div>

        <div className="grid gap-2 max-h-96 overflow-y-auto">
          {pacientes.map((paciente) => (
            <div
              key={paciente.id}
              className="flex items-center justify-between p-3 border rounded-lg hover:bg-gray-50"
            >
              <div className="flex-1">
                <div className="flex items-center gap-2">
                  <span className="text-lg">{getGeneroIcon(paciente.genero)}</span>
                  <div>
                    <p className="font-medium">{paciente.nome_completo}</p>
                    <p className="text-sm text-gray-500">
                      {paciente.municipio} | CPF: {paciente.cpf}
                    </p>
                  </div>
                </div>
                <div className="mt-2 flex items-center gap-2">
                  <Badge className={getGeneroBadgeColor(paciente.genero)}>
                    {paciente.genero}
                  </Badge>
                  {paciente.convenio ? (
                    <Badge className="bg-green-100 text-green-800">
                      Com Convênio
                    </Badge>
                  ) : (
                    <Badge variant="outline" className="text-red-600 border-red-300">
                      Sem Convênio
                    </Badge>
                  )}
                </div>
              </div>
              <div className="text-right text-xs text-gray-500 min-w-0">
                <p className="font-medium">CID-10: {paciente.cid10}</p>
                <p className="truncate max-w-32" title={paciente.descricaoCid10}>
                  {paciente.descricaoCid10}
                </p>
              </div>
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  );
}