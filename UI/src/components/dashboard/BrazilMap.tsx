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

  const getColor = (value: number) => {
    const intensity = value / maxValue;
    if (intensity > 0.8) return "hsl(var(--chart-1))";
    if (intensity > 0.6) return "hsl(var(--chart-2))";
    if (intensity > 0.4) return "hsl(var(--chart-3))";
    if (intensity > 0.2) return "hsl(var(--chart-4))";
    return "hsl(var(--muted))";
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
                style={{ backgroundColor: getColor(state.value) }}
                onClick={() => setSelectedState(selectedState === state.id ? null : state.id)}
              >
                <div className="text-sm font-medium text-white">{state.id}</div>
                <div className="text-xs text-white/80">{state.value.toLocaleString()}</div>
              </div>
            ))}
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