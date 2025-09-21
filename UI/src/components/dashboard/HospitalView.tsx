import { KPICard } from "./KPICard";
import { ChartCard } from "./ChartCard";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from "recharts";
import { Building2, Users, Bed, TrendingUp, AlertCircle } from "lucide-react";
import { useState } from "react";
import { useHospitalsList, useHospitalDetails, useHospitalSpecialties } from "@/hooks/useGetData";
import { Skeleton } from "@/components/ui/skeleton";
import { Alert, AlertDescription } from "@/components/ui/alert";

export const HospitalView = () => {
  const [selectedHospitalId, setSelectedHospitalId] = useState<number | null>(null);
  
  const { data: hospitalsList, loading: hospitalsLoading, error: hospitalsError } = useHospitalsList();
  const { data: hospitalDetails, loading: detailsLoading, error: detailsError } = useHospitalDetails(selectedHospitalId || 0);
  const { data: hospitalSpecialties, loading: specialtiesLoading, error: specialtiesError } = useHospitalSpecialties(selectedHospitalId || 0);

  // Selecionar primeiro hospital por padrão
  if (!selectedHospitalId && hospitalsList.length > 0) {
    setSelectedHospitalId(hospitalsList[0].id);
  }

  if (hospitalsLoading) {
    return (
      <div className="space-y-8">
        <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
          <div>
            <h2 className="text-3xl font-bold text-foreground mb-2">Análise por Hospital</h2>
            <p className="text-muted-foreground">Carregando dados...</p>
          </div>
          <Skeleton className="w-64 h-10" />
        </div>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {[...Array(4)].map((_, i) => (
            <Skeleton key={i} className="h-32 w-full" />
          ))}
        </div>
      </div>
    );
  }

  if (hospitalsError) {
    return (
      <div className="space-y-8">
        <div>
          <h2 className="text-3xl font-bold text-foreground mb-2">Análise por Hospital</h2>
          <p className="text-muted-foreground">Performance e carga de trabalho hospitalar</p>
        </div>
        <Alert>
          <AlertCircle className="h-4 w-4" />
          <AlertDescription>
            Erro ao carregar hospitais: {hospitalsError}
          </AlertDescription>
        </Alert>
      </div>
    );
  }

  if (hospitalsList.length === 0) {
    return (
      <div className="space-y-8">
        <div>
          <h2 className="text-3xl font-bold text-foreground mb-2">Análise por Hospital</h2>
          <p className="text-muted-foreground">Performance e carga de trabalho hospitalar</p>
        </div>
        <Alert>
          <AlertCircle className="h-4 w-4" />
          <AlertDescription>
            Nenhum hospital encontrado na base de dados.
          </AlertDescription>
        </Alert>
      </div>
    );
  }

  return (
    <div className="space-y-8">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h2 className="text-3xl font-bold text-foreground mb-2">Análise por Hospital</h2>
          <p className="text-muted-foreground">Performance e carga de trabalho hospitalar</p>
        </div>
        
        <div className="flex gap-4">
          <Select 
            value={selectedHospitalId?.toString() || ""} 
            onValueChange={(value) => setSelectedHospitalId(Number(value))}
          >
            <SelectTrigger className="w-64">
              <SelectValue placeholder="Selecione um hospital" />
            </SelectTrigger>
            <SelectContent>
              {hospitalsList.map((hospital) => (
                <SelectItem key={hospital.id} value={hospital.id.toString()}>
                  {hospital.nome} - {hospital.cidade}/{hospital.uf}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      {detailsLoading ? (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {[...Array(4)].map((_, i) => (
            <Skeleton key={i} className="h-32 w-full" />
          ))}
        </div>
      ) : detailsError ? (
        <Alert>
          <AlertCircle className="h-4 w-4" />
          <AlertDescription>
            Erro ao carregar detalhes do hospital: {detailsError}
          </AlertDescription>
        </Alert>
      ) : hospitalDetails ? (
        <>
          {/* Hospital KPIs */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            <KPICard
              title="Taxa de Ocupação"
              value={`${hospitalDetails.taxaOcupacao}%`}
              change={hospitalDetails.taxaOcupacao > 80 ? "Alta ocupação" : "Ocupação normal"}
              trend={hospitalDetails.taxaOcupacao > 80 ? "up" : "neutral"}
              icon={Bed}
            />
            <KPICard
              title="Capacidade Total"
              value={hospitalDetails.leitosTotal.toString()}
              change={`${hospitalDetails.leitosOcupados} ocupados`}
              trend="neutral"
              icon={Building2}
            />
            <KPICard
              title="Médicos Alocados"
              value={hospitalDetails.medicosAlocados.toString()}
              change={`${(hospitalDetails.leitosTotal / hospitalDetails.medicosAlocados).toFixed(1)} leitos/médico`}
              trend="neutral"
              icon={Users}
            />
            <KPICard
              title="Ranking Regional"
              value={`${hospitalDetails.rankingRegional}º`}
              change="Entre hospitais do estado"
              trend="up"
              icon={TrendingUp}
            />
          </div>

          {/* Hospital Details */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <Card className="bg-gradient-card shadow-card border-0">
              <CardHeader>
                <CardTitle className="text-lg font-semibold text-foreground">
                  Informações do Hospital
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div>
                  <h3 className="font-semibold text-foreground">{hospitalDetails.nome}</h3>
                  <p className="text-sm text-muted-foreground">{hospitalDetails.cidade}, {hospitalDetails.uf}</p>
                </div>
                
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">Leitos Totais</p>
                    <p className="text-xl font-bold text-foreground">{hospitalDetails.leitosTotal}</p>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">Leitos Ocupados</p>
                    <p className="text-xl font-bold text-foreground">{hospitalDetails.leitosOcupados}</p>
                  </div>
                </div>

                <div className="w-full bg-muted rounded-full h-2">
                  <div 
                    className="bg-primary h-2 rounded-full transition-all duration-300"
                    style={{ width: `${hospitalDetails.taxaOcupacao}%` }}
                  />
                </div>
              </CardContent>
            </Card>

            <ChartCard title="Médicos por Especialidade no Hospital">
              {specialtiesLoading ? (
                <div className="flex items-center justify-center h-72">
                  <Skeleton className="w-full h-full" />
                </div>
              ) : specialtiesError ? (
                <div className="flex items-center justify-center h-72 text-muted-foreground">
                  <div className="text-center">
                    <AlertCircle className="w-8 h-8 mx-auto mb-2" />
                    <p>Erro ao carregar especialidades</p>
                  </div>
                </div>
              ) : hospitalSpecialties.length > 0 ? (
                <ResponsiveContainer width="100%" height={300}>
                  <BarChart data={hospitalSpecialties.slice(0, 4)}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="especialidade" angle={-45} textAnchor="end" height={80} />
                    <YAxis />
                    <Tooltip />
                    <Bar dataKey="numeroMedicos" fill="hsl(var(--chart-2))" />
                  </BarChart>
                </ResponsiveContainer>
              ) : (
                <div className="flex items-center justify-center h-72 text-muted-foreground">
                  <div className="text-center">
                    <Users className="w-8 h-8 mx-auto mb-2" />
                    <p>Nenhuma especialidade encontrada</p>
                  </div>
                </div>
              )}
            </ChartCard>
          </div>
        </>
      ) : (
        <Alert>
          <AlertCircle className="h-4 w-4" />
          <AlertDescription>
            Selecione um hospital para visualizar os detalhes.
          </AlertDescription>
        </Alert>
      )}
    </div>
  );
};