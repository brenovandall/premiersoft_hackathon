import { KPICard } from "./KPICard";
import { ChartCard } from "./ChartCard";
import { BrazilMap } from "./BrazilMap";
import { DiseasesByRegion } from "./DiseasesByRegion";
import { HospitalizationsByLocation } from "./HospitalizationsByLocation";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
  LineChart,
  Line,
} from "recharts";
import { Users, Hospital, UserCheck, Activity, MapPin, Building } from "lucide-react";
import { Skeleton } from "@/components/ui/skeleton";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { AlertCircle } from "lucide-react";
import {
  useRealDataKPIs,
  useRealDiseaseData,
  useRealSpecialtyData,
} from "@/hooks/useRealData";

const COLORS = [
  "hsl(var(--chart-1))",
  "hsl(var(--chart-2))",
  "hsl(var(--chart-3))",
  "hsl(var(--chart-4))",
  "hsl(var(--chart-5))",
];

export const OverviewView = () => {
  const kpiData = useRealDataKPIs();
  const diseaseData = useRealDiseaseData();
  const specialtyData = useRealSpecialtyData();

  if (kpiData.loading) {
    return (
      <div className="space-y-8">
        <div>
          <h2 className="text-3xl font-bold text-foreground mb-2">
            Visão Geral do Sistema
          </h2>
          <p className="text-muted-foreground">
            Carregando dados do sistema...
          </p>
        </div>
        
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {[...Array(6)].map((_, i) => (
            <Skeleton key={i} className="h-32 w-full" />
          ))}
        </div>
        
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <Skeleton className="h-96 w-full" />
          <Skeleton className="h-96 w-full" />
        </div>
      </div>
    );
  }

  if (kpiData.error) {
    return (
      <div className="space-y-8">
        <div>
          <h2 className="text-3xl font-bold text-foreground mb-2">
            Visão Geral do Sistema
          </h2>
          <p className="text-muted-foreground">
            Panorama nacional da saúde brasileira
          </p>
        </div>
        
        <Alert>
          <AlertCircle className="h-4 w-4" />
          <AlertDescription>
            Erro ao carregar dados: {kpiData.error}
          </AlertDescription>
        </Alert>
      </div>
    );
  }

  return (
    <div className="space-y-8">
      <div>
        <h2 className="text-3xl font-bold text-foreground mb-2">
          Visão Geral do Sistema
        </h2>
        <p className="text-muted-foreground">
          Panorama nacional da saúde brasileira com dados reais
        </p>
      </div>

      {/* KPI Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        <KPICard
          title="Total de Pacientes"
          value={kpiData.totalPatients.toLocaleString()}
          trend="up"
          icon={Users}
        />
        <KPICard
          title="Hospitais Cadastrados"
          value={kpiData.totalHospitals.toLocaleString()}
          trend="up"
          icon={Hospital}
        />
        <KPICard
          title="Médicos Cadastrados"
          value={kpiData.totalDoctors.toLocaleString()}
          trend="up"
          icon={UserCheck}
        />
        <KPICard
          title="Leitos por Hospital"
          value={kpiData.averageBedsPerHospital.toLocaleString()}
          trend="up"
          icon={Activity}
        />
      </div>

      {/* Charts Row */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <BrazilMap />

        <ChartCard title="Principais Doenças (CID-10)">
          {diseaseData.length > 0 ? (
            <ResponsiveContainer width="100%" height={400}>
              <PieChart>
                <Pie
                  data={diseaseData}
                  cx="50%"
                  cy="50%"
                  labelLine={false}
                  label={({ name, percent }) =>
                    `${name} ${(percent * 100).toFixed(0)}%`
                  }
                  outerRadius={120}
                  fill="#8884d8"
                  dataKey="value"
                >
                  {diseaseData.map((entry, index) => (
                    <Cell
                      key={`cell-${index}`}
                      fill={COLORS[index % COLORS.length]}
                    />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          ) : (
            <div className="flex items-center justify-center h-96 text-muted-foreground">
              Nenhum dado de doenças disponível
            </div>
          )}
        </ChartCard>
      </div>

      {/* Additional Charts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <ChartCard title="Médicos por Especialidade">
          {specialtyData.length > 0 ? (
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={specialtyData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis 
                  dataKey="specialty" 
                  angle={-45}
                  textAnchor="end"
                  height={100}
                />
                <YAxis />
                <Tooltip />
                <Bar dataKey="doctors" fill={COLORS[0]} />
              </BarChart>
            </ResponsiveContainer>
          ) : (
            <div className="flex items-center justify-center h-72 text-muted-foreground">
              Nenhum dado de especialidades disponível
            </div>
          )}
        </ChartCard>

        <ChartCard title="Distribuição de Convênios">
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={[
                  { name: 'Com Convênio', value: kpiData.patientsWithInsurance },
                  { name: 'Sem Convênio', value: kpiData.patientsWithoutInsurance }
                ]}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={({ name, percent }) =>
                  `${name} ${(percent * 100).toFixed(1)}%`
                }
                outerRadius={100}
                fill="#8884d8"
                dataKey="value"
              >
                <Cell fill="hsl(var(--chart-1))" />
                <Cell fill="hsl(var(--chart-2))" />
              </Pie>
              <Tooltip />
            </PieChart>
          </ResponsiveContainer>
        </ChartCard>
      </div>

      {/* New Analysis Section */}
      <div className="space-y-6">
        <div>
          <h3 className="text-xl font-semibold text-foreground mb-4">
            Análise Regional e Epidemiológica
          </h3>
        </div>

        {/* Regional Disease Analysis and Location-based Hospitalizations */}
        <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">
          <DiseasesByRegion />
          <HospitalizationsByLocation />
        </div>
      </div>
    </div>
  );
};
