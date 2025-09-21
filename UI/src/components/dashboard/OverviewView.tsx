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
import { Users, Hospital, UserCheck, Activity } from "lucide-react";
import {
  mockKPIData,
  mockDiseaseData,
  mockSpecialtyData,
  mockMonthlyData,
} from "@/data/mockData";

const COLORS = [
  "hsl(var(--chart-1))",
  "hsl(var(--chart-2))",
  "hsl(var(--chart-3))",
  "hsl(var(--chart-4))",
  "hsl(var(--chart-5))",
];

export const OverviewView = () => {
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

      {/* KPI Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <KPICard
          title="Total de Pacientes"
          value={mockKPIData.totalPatients.toLocaleString()}
          trend="up"
          icon={Users}
        />
        <KPICard
          title="Hospitais Ativos"
          value={mockKPIData.totalHospitals.toLocaleString()}
          trend="up"
          icon={Hospital}
        />
        <KPICard
          title="Médicos Cadastrados"
          value={mockKPIData.totalDoctors.toLocaleString()}
          trend="up"
          icon={UserCheck}
        />
        <KPICard
          title="Ocupação Média"
          value={`${mockKPIData.averageOccupancy}%`}
          trend="down"
          icon={Activity}
        />
      </div>

      {/* Charts Row */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <BrazilMap />

        <ChartCard title="Principais Doenças (CID-10)">
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={mockDiseaseData}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={({ name, percent }) =>
                  `${name} ${(percent * 100).toFixed(0)}%`
                }
                outerRadius={80}
                fill="#8884d8"
                dataKey="value"
              >
                {mockDiseaseData.map((entry, index) => (
                  <Cell
                    key={`cell-${index}`}
                    fill={COLORS[index % COLORS.length]}
                  />
                ))}
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
