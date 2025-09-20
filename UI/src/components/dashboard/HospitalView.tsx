import { KPICard } from "./KPICard";
import { ChartCard } from "./ChartCard";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from "recharts";
import { Building2, Users, Bed, TrendingUp } from "lucide-react";
import { mockHospitalData, mockSpecialtyData } from "@/data/mockData";
import { useState } from "react";

export const HospitalView = () => {
  const [selectedHospital, setSelectedHospital] = useState("1");
  const hospital = mockHospitalData.find(h => h.id.toString() === selectedHospital) || mockHospitalData[0];

  return (
    <div className="space-y-8">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h2 className="text-3xl font-bold text-foreground mb-2">Análise por Hospital</h2>
          <p className="text-muted-foreground">Performance e carga de trabalho hospitalar</p>
        </div>
        
        <div className="flex gap-4">
          <Select value={selectedHospital} onValueChange={setSelectedHospital}>
            <SelectTrigger className="w-64">
              <SelectValue placeholder="Selecione um hospital" />
            </SelectTrigger>
            <SelectContent>
              {mockHospitalData.map((hospital) => (
                <SelectItem key={hospital.id} value={hospital.id.toString()}>
                  {hospital.name} - {hospital.city}/{hospital.state}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      {/* Hospital KPIs */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <KPICard
          title="Taxa de Ocupação"
          value={`${hospital.occupancy}%`}
          change={hospital.occupancy > 80 ? "Alta ocupação" : "Ocupação normal"}
          trend={hospital.occupancy > 80 ? "up" : "neutral"}
          icon={Bed}
        />
        <KPICard
          title="Capacidade Total"
          value={hospital.capacity.toString()}
          change={`${Math.round(hospital.capacity * hospital.occupancy / 100)} ocupados`}
          trend="neutral"
          icon={Building2}
        />
        <KPICard
          title="Médicos Alocados"
          value={hospital.doctors.toString()}
          change={`${(hospital.capacity / hospital.doctors).toFixed(1)} leitos/médico`}
          trend="neutral"
          icon={Users}
        />
        <KPICard
          title="Ranking Regional"
          value="3º"
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
              <h3 className="font-semibold text-foreground">{hospital.name}</h3>
              <p className="text-sm text-muted-foreground">{hospital.city}, {hospital.state}</p>
            </div>
            
            <div className="grid grid-cols-2 gap-4">
              <div>
                <p className="text-sm font-medium text-muted-foreground">Leitos Totais</p>
                <p className="text-xl font-bold text-foreground">{hospital.capacity}</p>
              </div>
              <div>
                <p className="text-sm font-medium text-muted-foreground">Leitos Ocupados</p>
                <p className="text-xl font-bold text-foreground">
                  {Math.round(hospital.capacity * hospital.occupancy / 100)}
                </p>
              </div>
            </div>

            <div className="w-full bg-muted rounded-full h-2">
              <div 
                className="bg-primary h-2 rounded-full transition-all duration-300"
                style={{ width: `${hospital.occupancy}%` }}
              />
            </div>
          </CardContent>
        </Card>

        <ChartCard title="Médicos por Especialidade no Hospital">
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={mockSpecialtyData.slice(0, 4)}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="specialty" angle={-45} textAnchor="end" height={80} />
              <YAxis />
              <Tooltip />
              <Bar dataKey="doctors" fill="hsl(var(--chart-2))" />
            </BarChart>
          </ResponsiveContainer>
        </ChartCard>
      </div>

      {/* Hospital Comparison */}
      <ChartCard title="Comparação entre Hospitais" className="w-full">
        <ResponsiveContainer width="100%" height={400}>
          <BarChart data={mockHospitalData}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="name" angle={-45} textAnchor="end" height={100} />
            <YAxis yAxisId="left" />
            <YAxis yAxisId="right" orientation="right" />
            <Tooltip />
            <Bar yAxisId="left" dataKey="doctors" fill="hsl(var(--chart-1))" name="Médicos" />
            <Bar yAxisId="right" dataKey="occupancy" fill="hsl(var(--chart-3))" name="Ocupação %" />
          </BarChart>
        </ResponsiveContainer>
      </ChartCard>
    </div>
  );
};