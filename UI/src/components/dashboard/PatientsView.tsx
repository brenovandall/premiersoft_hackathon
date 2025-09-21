import { KPICard } from "./KPICard";
import { ChartCard } from "./ChartCard";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from "recharts";
import { Users, UserCheck, Search, Filter, AlertCircle } from "lucide-react";
import { useState } from "react";
import { usePatientDemographics, usePatientStats, useDoctorSpecialtyStats, useDoctorSearch } from "@/hooks/useGetData";
import { Skeleton } from "@/components/ui/skeleton";
import { Alert, AlertDescription } from "@/components/ui/alert";

export const PatientsView = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedSpecialty, setSelectedSpecialty] = useState("all");

  // API hooks
  const { data: demographicsData, loading: demographicsLoading, error: demographicsError } = usePatientDemographics();
  const { data: patientStats, loading: statsLoading, error: statsError } = usePatientStats();
  const { data: specialtyStats, loading: specialtyLoading, error: specialtyError } = useDoctorSpecialtyStats();
  const { data: doctors, loading: doctorsLoading, error: doctorsError } = useDoctorSearch(searchTerm, selectedSpecialty === "all" ? undefined : selectedSpecialty);

  // Calculate totals from demographics data
  const totalPatients = demographicsData?.reduce((sum, item) => sum + item.totalCount, 0) || 0;
  const malePatients = demographicsData?.reduce((sum, item) => sum + item.maleCount, 0) || 0;
  const femalePatients = demographicsData?.reduce((sum, item) => sum + item.femaleCount, 0) || 0;

  // Transform specialty stats for chart
  const chartData = specialtyStats?.map(stat => ({
    specialty: stat.especialidade,
    patients: stat.totalPacientes,
    doctors: stat.totalMedicos
  })) || [];

  // Get unique specialties for filter
  const uniqueSpecialties = [...new Set(specialtyStats?.map(stat => stat.especialidade) || [])];

  return (
    <div className="space-y-8">
      <div>
        <h2 className="text-3xl font-bold text-foreground mb-2">Pacientes & Médicos</h2>
        <p className="text-muted-foreground">Análise demográfica e distribuição de profissionais</p>
      </div>

      {/* Patient Demographics KPIs */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {statsLoading ? (
          <>
            <Skeleton className="h-24" />
            <Skeleton className="h-24" />
            <Skeleton className="h-24" />
          </>
        ) : statsError ? (
          <div className="col-span-3">
            <Alert variant="destructive">
              <AlertCircle className="h-4 w-4" />
              <AlertDescription>{statsError}</AlertDescription>
            </Alert>
          </div>
        ) : (
          <>
            <KPICard
              title="Total de Pacientes"
              value={patientStats?.totalPacientes?.toLocaleString() || '0'}
              change="Todas as idades"
              trend="neutral"
              icon={Users}
            />
            <KPICard
              title="Pacientes Masculinos"
              value={malePatients.toLocaleString()}
              change={totalPatients > 0 ? `${((malePatients / totalPatients) * 100).toFixed(1)}%` : '0%'}
              trend="neutral"
              icon={UserCheck}
            />
            <KPICard
              title="Pacientes Femininas"
              value={femalePatients.toLocaleString()}
              change={totalPatients > 0 ? `${((femalePatients / totalPatients) * 100).toFixed(1)}%` : '0%'}
              trend="neutral"
              icon={UserCheck}
            />
          </>
        )}
      </div>

      {/* Charts */}
      <div className="grid grid-cols-1 lg:grid-cols-1 gap-6">
        <ChartCard title="Relação Médicos/Pacientes por Especialidade">
          {specialtyLoading ? (
            <Skeleton className="h-[300px]" />
          ) : specialtyError ? (
            <Alert variant="destructive">
              <AlertCircle className="h-4 w-4" />
              <AlertDescription>{specialtyError}</AlertDescription>
            </Alert>
          ) : chartData.length === 0 ? (
            <div className="h-[300px] flex items-center justify-center text-muted-foreground">
              Nenhum dado de especialidade disponível
            </div>
          ) : (
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={chartData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="specialty" angle={-45} textAnchor="end" height={80} />
                <YAxis />
                <Tooltip />
                <Bar dataKey="patients" fill="hsl(var(--chart-3))" name="Pacientes" />
              </BarChart>
            </ResponsiveContainer>
          )}
        </ChartCard>
      </div>

      {/* Doctor Search */}
      <Card className="bg-gradient-card shadow-card border-0">
        <CardHeader>
          <CardTitle className="text-lg font-semibold text-foreground flex items-center gap-2">
            <Search className="h-5 w-5" />
            Busca de Médicos
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex flex-col md:flex-row gap-4">
            <div className="flex-1">
              <Input
                placeholder="Buscar médico por nome..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full"
              />
            </div>
            <div className="w-full md:w-64">
              <Select value={selectedSpecialty} onValueChange={setSelectedSpecialty}>
                <SelectTrigger>
                  <Filter className="h-4 w-4 mr-2" />
                  <SelectValue placeholder="Filtrar por especialidade" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">Todas as especialidades</SelectItem>
                  {uniqueSpecialties.map((specialty) => (
                    <SelectItem key={specialty} value={specialty}>
                      {specialty}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          {doctorsLoading ? (
            <div className="space-y-3">
              <Skeleton className="h-20" />
              <Skeleton className="h-20" />
              <Skeleton className="h-20" />
            </div>
          ) : doctorsError ? (
            <Alert variant="destructive">
              <AlertCircle className="h-4 w-4" />
              <AlertDescription>{doctorsError}</AlertDescription>
            </Alert>
          ) : (
            <div className="space-y-3">
              {doctors.map((doctor) => (
                <div key={doctor.id} className="p-4 bg-accent rounded-lg">
                  <div className="flex justify-between items-start">
                    <div>
                      <h3 className="font-semibold text-accent-foreground">{doctor.nome}</h3>
                      <p className="text-sm text-accent-foreground/80">{doctor.especialidade}</p>
                    </div>
                    <div className="text-right">
                      <p className="text-sm font-medium text-accent-foreground">
                        CRM: {doctor.crm}
                      </p>
                    </div>
                  </div>
                  <div className="mt-2">
                    <p className="text-xs text-accent-foreground/70">
                      Total de pacientes: {doctor.totalPacientes || 0}
                    </p>
                  </div>
                </div>
              ))}
            </div>
          )}

          {!doctorsLoading && !doctorsError && doctors.length === 0 && (
            <div className="text-center py-8">
              <p className="text-muted-foreground">Nenhum médico encontrado com os critérios selecionados.</p>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
};