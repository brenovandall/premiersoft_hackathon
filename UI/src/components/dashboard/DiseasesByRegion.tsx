import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Cell } from "recharts";
import { Skeleton } from "@/components/ui/skeleton";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { AlertCircle } from "lucide-react";
import { useRealRegionData } from "@/hooks/useRealData";

const diseasesByRegion = [
  {
    region: "Norte",
    diseases: [
      { name: "Malária", cases: 1250, color: "#dc2626" },
      { name: "Dengue", cases: 980, color: "#f87171" },
      { name: "Tuberculose", cases: 750, color: "#fbbf24" },
      { name: "Pneumonia", cases: 650, color: "#a3e635" },
      { name: "Diarreia", cases: 540, color: "#22c55e" },
    ]
  },
  {
    region: "Nordeste",
    diseases: [
      { name: "Dengue", cases: 2100, color: "#dc2626" },
      { name: "Chikungunya", cases: 1800, color: "#f87171" },
      { name: "Zika", cases: 1200, color: "#fbbf24" },
      { name: "Pneumonia", cases: 950, color: "#a3e635" },
      { name: "Tuberculose", cases: 820, color: "#22c55e" },
    ]
  },
  {
    region: "Centro-Oeste",
    diseases: [
      { name: "Dengue", cases: 890, color: "#dc2626" },
      { name: "Febre Amarela", cases: 650, color: "#f87171" },
      { name: "Pneumonia", cases: 580, color: "#fbbf24" },
      { name: "Malária", cases: 420, color: "#a3e635" },
      { name: "Tuberculose", cases: 380, color: "#22c55e" },
    ]
  },
  {
    region: "Sudeste",
    diseases: [
      { name: "COVID-19", cases: 3200, color: "#dc2626" },
      { name: "Pneumonia", cases: 2800, color: "#f87171" },
      { name: "Dengue", cases: 1900, color: "#fbbf24" },
      { name: "Influenza", cases: 1500, color: "#a3e635" },
      { name: "Tuberculose", cases: 1200, color: "#22c55e" },
    ]
  },
  {
    region: "Sul",
    diseases: [
      { name: "COVID-19", cases: 1800, color: "#dc2626" },
      { name: "Pneumonia", cases: 1600, color: "#f87171" },
      { name: "Influenza", cases: 1200, color: "#fbbf24" },
      { name: "Tuberculose", cases: 800, color: "#a3e635" },
      { name: "Dengue", cases: 650, color: "#22c55e" },
    ]
  },
];

export const DiseasesByRegion = () => {
  const regionData = useRealRegionData();

  if (regionData.length === 0) {
    return (
      <Card className="bg-gradient-card shadow-card border-0">
        <CardHeader>
          <CardTitle className="text-lg font-semibold text-foreground">
            Dados por Região
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {[...Array(5)].map((_, i) => (
              <div key={i} className="space-y-2">
                <Skeleton className="h-4 w-32" />
                <Skeleton className="h-32 w-full" />
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className="bg-gradient-card shadow-card border-0">
      <CardHeader>
        <CardTitle className="text-lg font-semibold text-foreground">
          Distribuição por Região
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div className="space-y-6">
          <div className="h-64">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart
                data={regionData}
                margin={{ top: 20, right: 30, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" className="opacity-30" />
                <XAxis 
                  dataKey="regiao" 
                  tick={{ fontSize: 12 }}
                />
                <YAxis tick={{ fontSize: 12 }} />
                <Tooltip 
                  formatter={(value, name) => {
                    const labels: Record<string, string> = {
                      pacientes: 'Pacientes',
                      medicos: 'Médicos',
                      hospitais: 'Hospitais',
                      municipios: 'Municípios'
                    };
                    return [value, labels[name as string] || name];
                  }}
                  labelStyle={{ color: 'hsl(var(--foreground))' }}
                  contentStyle={{ 
                    backgroundColor: 'hsl(var(--background))', 
                    border: '1px solid hsl(var(--border))',
                    borderRadius: '8px'
                  }}
                />
                <Bar dataKey="pacientes" fill="#dc2626" radius={[2, 2, 0, 0]} />
                <Bar dataKey="medicos" fill="#16a34a" radius={[2, 2, 0, 0]} />
                <Bar dataKey="hospitais" fill="#2563eb" radius={[2, 2, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
          
          {/* Tabela de resumo */}
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b">
                  <th className="text-left py-2">Região</th>
                  <th className="text-right py-2">Estados</th>
                  <th className="text-right py-2">Municípios</th>
                  <th className="text-right py-2">Pacientes</th>
                  <th className="text-right py-2">Médicos</th>
                  <th className="text-right py-2">Hospitais</th>
                </tr>
              </thead>
              <tbody>
                {regionData.map((region) => (
                  <tr key={region.regiao} className="border-b">
                    <td className="py-2 font-medium">{region.regiao}</td>
                    <td className="text-right py-2">{region.estados}</td>
                    <td className="text-right py-2">{region.municipios.toLocaleString()}</td>
                    <td className="text-right py-2">{region.pacientes.toLocaleString()}</td>
                    <td className="text-right py-2">{region.medicos.toLocaleString()}</td>
                    <td className="text-right py-2">{region.hospitais.toLocaleString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </CardContent>
    </Card>
  );
};