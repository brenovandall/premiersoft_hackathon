import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useState } from "react";
import { useRealStateData } from "@/hooks/useRealData";
import { Skeleton } from "@/components/ui/skeleton";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { AlertCircle } from "lucide-react";

export const BrazilMap = () => {
  const [selectedState, setSelectedState] = useState<string | null>(null);
  const stateData = useRealStateData();

  // Transformar dados da API para o formato esperado
  const states = stateData.map(state => ({
    id: state.uf,
    name: state.estado,
    value: state.pacientes
  }));

  if (states.length === 0) {
    return (
      <Card className="bg-gradient-card shadow-card border-0">
        <CardHeader>
          <CardTitle className="text-lg font-semibold text-foreground">
            Internações por Estado
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex items-center justify-center h-40 text-muted-foreground">
            <div className="text-center">
              <AlertCircle className="w-8 h-8 mx-auto mb-2" />
              <p>Nenhum dado disponível</p>
              <p className="text-sm">Verifique se há dados na base de dados</p>
            </div>
          </div>
        </CardContent>
      </Card>
    );
  }

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