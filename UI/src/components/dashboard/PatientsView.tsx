import { KPICard } from "./KPICard";
import { ChartCard } from "./ChartCard";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from "recharts";
import { Users, UserCheck, Search, Filter } from "lucide-react";
import { mockAgeGenderData, mockSpecialtyData } from "@/data/mockData";
import { useState } from "react";

const mockDoctors = [
  { id: 1, name: "Dr. João Silva", specialty: "Cardiologia", hospitals: ["Hospital das Clínicas", "Albert Einstein"] },
  { id: 2, name: "Dra. Maria Santos", specialty: "Pneumologia", hospitals: ["Sírio-Libanês"] },
  { id: 3, name: "Dr. Pedro Costa", specialty: "Endocrinologia", hospitals: ["Copa D'Or", "Hospital das Clínicas"] },
  { id: 4, name: "Dra. Ana Lima", specialty: "Neurologia", hospitals: ["Albert Einstein", "Sírio-Libanês", "Copa D'Or"] },
];

export const PatientsView = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedSpecialty, setSelectedSpecialty] = useState("all");

  const filteredDoctors = mockDoctors.filter(doctor => {
    const matchesSearch = doctor.name.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesSpecialty = selectedSpecialty === "all" || doctor.specialty === selectedSpecialty;
    return matchesSearch && matchesSpecialty;
  });

  const totalPatients = mockAgeGenderData.reduce((sum, item) => sum + item.male + item.female, 0);
  const malePatients = mockAgeGenderData.reduce((sum, item) => sum + item.male, 0);
  const femalePatients = mockAgeGenderData.reduce((sum, item) => sum + item.female, 0);

  return (
    <div className="space-y-8">
      <div>
        <h2 className="text-3xl font-bold text-foreground mb-2">Pacientes & Médicos</h2>
        <p className="text-muted-foreground">Análise demográfica e distribuição de profissionais</p>
      </div>

      {/* Patient Demographics KPIs */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <KPICard
          title="Total de Pacientes"
          value={totalPatients.toLocaleString()}
          change="Todas as idades"
          trend="neutral"
          icon={Users}
        />
        <KPICard
          title="Pacientes Masculinos"
          value={malePatients.toLocaleString()}
          change={`${((malePatients / totalPatients) * 100).toFixed(1)}%`}
          trend="neutral"
          icon={UserCheck}
        />
        <KPICard
          title="Pacientes Femininas"
          value={femalePatients.toLocaleString()}
          change={`${((femalePatients / totalPatients) * 100).toFixed(1)}%`}
          trend="neutral"
          icon={UserCheck}
        />
        <KPICard
          title="Idade Média"
          value="52 anos"
          change="Baseado em atendimentos"
          trend="neutral"
          icon={Users}
        />
      </div>

      {/* Charts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <ChartCard title="Distribuição por Idade e Gênero">
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={mockAgeGenderData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="ageGroup" />
              <YAxis />
              <Tooltip />
              <Bar dataKey="male" fill="hsl(var(--chart-1))" name="Masculino" />
              <Bar dataKey="female" fill="hsl(var(--chart-2))" name="Feminino" />
            </BarChart>
          </ResponsiveContainer>
        </ChartCard>

        <ChartCard title="Relação Médicos/Pacientes por Especialidade">
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={mockSpecialtyData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="specialty" angle={-45} textAnchor="end" height={80} />
              <YAxis />
              <Tooltip />
              <Bar dataKey="patients" fill="hsl(var(--chart-3))" name="Pacientes" />
            </BarChart>
          </ResponsiveContainer>
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
                  <SelectItem value="Cardiologia">Cardiologia</SelectItem>
                  <SelectItem value="Pneumologia">Pneumologia</SelectItem>
                  <SelectItem value="Endocrinologia">Endocrinologia</SelectItem>
                  <SelectItem value="Neurologia">Neurologia</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="space-y-3">
            {filteredDoctors.map((doctor) => (
              <div key={doctor.id} className="p-4 bg-accent rounded-lg">
                <div className="flex justify-between items-start">
                  <div>
                    <h3 className="font-semibold text-accent-foreground">{doctor.name}</h3>
                    <p className="text-sm text-accent-foreground/80">{doctor.specialty}</p>
                  </div>
                  <div className="text-right">
                    <p className="text-sm font-medium text-accent-foreground">
                      {doctor.hospitals.length} hospital{doctor.hospitals.length > 1 ? "is" : ""}
                    </p>
                  </div>
                </div>
                <div className="mt-2">
                  <p className="text-xs text-accent-foreground/70">
                    Hospitais: {doctor.hospitals.join(", ")}
                  </p>
                </div>
              </div>
            ))}
          </div>

          {filteredDoctors.length === 0 && (
            <div className="text-center py-8">
              <p className="text-muted-foreground">Nenhum médico encontrado com os critérios selecionados.</p>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
};