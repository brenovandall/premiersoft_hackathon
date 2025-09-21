import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Cell } from "recharts";

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
  return (
    <Card className="bg-gradient-card shadow-card border-0">
      <CardHeader>
        <CardTitle className="text-lg font-semibold text-foreground">
          Doenças mais Incidentes por Região
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div className="space-y-6">
          {diseasesByRegion.map((regionData) => (
            <div key={regionData.region} className="space-y-3">
              <h3 className="text-base font-medium text-foreground border-b pb-2">
                Região {regionData.region}
              </h3>
              <div className="h-48">
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart
                    data={regionData.diseases}
                    margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
                  >
                    <CartesianGrid strokeDasharray="3 3" className="opacity-30" />
                    <XAxis 
                      dataKey="name" 
                      tick={{ fontSize: 12 }}
                      angle={-45}
                      textAnchor="end"
                      height={60}
                    />
                    <YAxis tick={{ fontSize: 12 }} />
                    <Tooltip 
                      formatter={(value) => [`${value} casos`, 'Casos']}
                      labelStyle={{ color: 'hsl(var(--foreground))' }}
                      contentStyle={{ 
                        backgroundColor: 'hsl(var(--background))', 
                        border: '1px solid hsl(var(--border))',
                        borderRadius: '8px'
                      }}
                    />
                    <Bar dataKey="cases" radius={[4, 4, 0, 0]}>
                      {regionData.diseases.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={entry.color} />
                      ))}
                    </Bar>
                  </BarChart>
                </ResponsiveContainer>
              </div>
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  );
};