import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useState } from "react";

const states = [
  { id: "AC", name: "Acre", value: 1250 },
  { id: "AL", name: "Alagoas", value: 3500 },
  { id: "AP", name: "Amapá", value: 890 },
  { id: "AM", name: "Amazonas", value: 5200 },
  { id: "BA", name: "Bahia", value: 12400 },
  { id: "CE", name: "Ceará", value: 8900 },
  { id: "DF", name: "Distrito Federal", value: 3200 },
  { id: "ES", name: "Espírito Santo", value: 4100 },
  { id: "GO", name: "Goiás", value: 6800 },
  { id: "MA", name: "Maranhão", value: 6200 },
  { id: "MT", name: "Mato Grosso", value: 3400 },
  { id: "MS", name: "Mato Grosso do Sul", value: 2800 },
  { id: "MG", name: "Minas Gerais", value: 18500 },
  { id: "PA", name: "Pará", value: 7800 },
  { id: "PB", name: "Paraíba", value: 3900 },
  { id: "PR", name: "Paraná", value: 11200 },
  { id: "PE", name: "Pernambuco", value: 9600 },
  { id: "PI", name: "Piauí", value: 3100 },
  { id: "RJ", name: "Rio de Janeiro", value: 16800 },
  { id: "RN", name: "Rio Grande do Norte", value: 3600 },
  { id: "RS", name: "Rio Grande do Sul", value: 11400 },
  { id: "RO", name: "Rondônia", value: 1800 },
  { id: "RR", name: "Roraima", value: 620 },
  { id: "SC", name: "Santa Catarina", value: 7200 },
  { id: "SP", name: "São Paulo", value: 42000 },
  { id: "SE", name: "Sergipe", value: 2300 },
  { id: "TO", name: "Tocantins", value: 1600 },
];

export const BrazilMap = () => {
  const [selectedState, setSelectedState] = useState<string | null>(null);
  const maxValue = Math.max(...states.map(s => s.value));

  const getTextColor = (value: number) => {
    const intensity = value / maxValue;
    return intensity > 0.4 ? "#ffffff" : "#000000";
  };

  const getColor = (value: number) => {
    const intensity = value / maxValue;
    // Escala de verde (baixo) para vermelho (alto)
    if (intensity > 0.8) return "#dc2626";
    if (intensity > 0.6) return "#f87171";
    if (intensity > 0.4) return "#fbbf24";
    if (intensity > 0.2) return "#a3e635";
    return "#22c55e"; // Verde
  };

  return (
    <Card className="bg-gradient-card shadow-card border-0">
      <CardHeader>
        <CardTitle className="text-lg font-semibold text-foreground">
          Internações por Estado
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div className="space-y-4">
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-2">
            {states.map((state) => (
              <div
                key={state.id}
                className="p-3 rounded-lg border cursor-pointer transition-smooth hover:shadow-md"
                style={{ 
                  backgroundColor: getColor(state.value),
                  color: getTextColor(state.value)
                }}
                onClick={() => setSelectedState(selectedState === state.id ? null : state.id)}
              >
                <div className="text-sm font-medium">{state.id}</div>
                <div className="text-xs opacity-80">{state.value.toLocaleString()}</div>
              </div>
            ))}
          </div>
          
          {/* Legenda de cores */}
          <div className="flex items-center justify-between text-xs text-muted-foreground">
            <div className="flex items-center gap-2">
              <div className="w-3 h-3 rounded" style={{ backgroundColor: "#22c55e" }}></div>
              <span>Menos internações</span>
            </div>
            <div className="flex items-center gap-2">
              <span>Mais internações</span>
              <div className="w-3 h-3 rounded" style={{ backgroundColor: "#dc2626" }}></div>
            </div>
          </div>
          
          {selectedState && (
            <div className="p-4 bg-accent rounded-lg">
              <h3 className="font-semibold text-accent-foreground">
                {states.find(s => s.id === selectedState)?.name}
              </h3>
              <p className="text-sm text-accent-foreground/80">
                Internações: {states.find(s => s.id === selectedState)?.value.toLocaleString()}
              </p>
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );
};