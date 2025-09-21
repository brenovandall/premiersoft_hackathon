import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useState } from "react";
import { ChevronDown, ChevronUp } from "lucide-react";
import { Skeleton } from "@/components/ui/skeleton";
import { useRealStateData } from "@/hooks/useRealData";

const hospitalizationData = [
  {
    state: "São Paulo",
    stateCode: "SP",
    total: 42000,
    municipalities: [
      { name: "São Paulo", cases: 18500 },
      { name: "Campinas", cases: 3200 },
      { name: "Santos", cases: 2800 },
      { name: "Ribeirão Preto", cases: 2100 },
      { name: "Sorocaba", cases: 1900 },
      { name: "Outros", cases: 13500 },
    ]
  },
  {
    state: "Rio de Janeiro",
    stateCode: "RJ",
    total: 16800,
    municipalities: [
      { name: "Rio de Janeiro", cases: 8900 },
      { name: "Niterói", cases: 1800 },
      { name: "Nova Iguaçu", cases: 1200 },
      { name: "Duque de Caxias", cases: 980 },
      { name: "São Gonçalo", cases: 850 },
      { name: "Outros", cases: 3070 },
    ]
  },
  {
    state: "Minas Gerais",
    stateCode: "MG",
    total: 18500,
    municipalities: [
      { name: "Belo Horizonte", cases: 6200 },
      { name: "Uberlândia", cases: 2100 },
      { name: "Contagem", cases: 1800 },
      { name: "Juiz de Fora", cases: 1500 },
      { name: "Betim", cases: 1200 },
      { name: "Outros", cases: 5700 },
    ]
  },
  {
    state: "Bahia",
    stateCode: "BA",
    total: 12400,
    municipalities: [
      { name: "Salvador", cases: 4800 },
      { name: "Feira de Santana", cases: 1900 },
      { name: "Vitória da Conquista", cases: 1200 },
      { name: "Camaçari", cases: 980 },
      { name: "Itabuna", cases: 750 },
      { name: "Outros", cases: 2770 },
    ]
  },
  {
    state: "Rio Grande do Sul",
    stateCode: "RS",
    total: 11400,
    municipalities: [
      { name: "Porto Alegre", cases: 3800 },
      { name: "Caxias do Sul", cases: 1600 },
      { name: "Pelotas", cases: 1200 },
      { name: "Canoas", cases: 980 },
      { name: "Santa Maria", cases: 850 },
      { name: "Outros", cases: 2970 },
    ]
  },
];

export const HospitalizationsByLocation = () => {
  const stateData = useRealStateData();
  const [expandedStates, setExpandedStates] = useState<Set<string>>(new Set());

  if (stateData.length === 0) {
    return (
      <Card className="bg-gradient-card shadow-card border-0">
        <CardHeader>
          <CardTitle className="text-lg font-semibold text-foreground">
            Dados por Estado
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {[...Array(5)].map((_, i) => (
              <Skeleton key={i} className="h-16 w-full" />
            ))}
          </div>
        </CardContent>
      </Card>
    );
  }

  const toggleExpanded = (state: string) => {
    const newExpanded = new Set(expandedStates);
    if (newExpanded.has(state)) {
      newExpanded.delete(state);
    } else {
      newExpanded.add(state);
    }
    setExpandedStates(newExpanded);
  };

  const maxPatients = Math.max(...stateData.map(s => s.pacientes));

  const getIntensityColor = (value: number, max: number) => {
    const intensity = value / max;
    if (intensity > 0.7) return "bg-red-500";
    if (intensity > 0.4) return "bg-yellow-400";
    return "bg-green-500";
  };

  const topStates = stateData.slice(0, 10); // Top 10 estados

  return (
    <Card className="bg-gradient-card shadow-card border-0">
      <CardHeader>
        <CardTitle className="text-lg font-semibold text-foreground">
          Distribuição por Estado (Top 10)
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div className="space-y-3 max-h-96 overflow-y-auto">
          {topStates.map((state) => {
            const isExpanded = expandedStates.has(state.estado);

            return (
              <div key={state.estado} className="border rounded-lg overflow-hidden bg-background">
                <div 
                  className="p-4 cursor-pointer hover:bg-accent/20 transition-colors"
                  onClick={() => toggleExpanded(state.estado)}
                >
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                      <div className={`w-3 h-3 rounded ${getIntensityColor(state.pacientes, maxPatients)}`}></div>
                      <div>
                        <h3 className="font-medium text-foreground">{state.estado} ({state.uf})</h3>
                        <p className="text-sm text-muted-foreground">
                          {state.municipios} municípios
                        </p>
                      </div>
                    </div>
                    <div className="text-right">
                      <p className="font-semibold text-lg text-foreground">
                        {state.pacientes.toLocaleString()}
                      </p>
                      <p className="text-xs text-muted-foreground">pacientes</p>
                    </div>
                    {isExpanded ? (
                      <ChevronUp className="w-4 h-4 text-muted-foreground ml-2" />
                    ) : (
                      <ChevronDown className="w-4 h-4 text-muted-foreground ml-2" />
                    )}
                  </div>
                </div>
                
                {isExpanded && (
                  <div className="px-4 pb-4 bg-accent/20">
                    <div className="grid grid-cols-2 gap-4 text-sm">
                      <div className="flex justify-between">
                        <span className="text-muted-foreground">Médicos:</span>
                        <span className="font-medium">{state.medicos.toLocaleString()}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-muted-foreground">Hospitais:</span>
                        <span className="font-medium">{state.hospitais.toLocaleString()}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-muted-foreground">Municípios:</span>
                        <span className="font-medium">{state.municipios.toLocaleString()}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-muted-foreground">Pacientes:</span>
                        <span className="font-medium">{state.pacientes.toLocaleString()}</span>
                      </div>
                    </div>
                  </div>
                )}
              </div>
            );
          })}
        </div>
        
        {/* Legenda */}
        <div className="mt-6 p-3 bg-accent/20 rounded-lg">
          <h4 className="text-sm font-medium text-foreground mb-2">Legenda de Intensidade:</h4>
          <div className="flex items-center gap-4 text-xs">
            <div className="flex items-center gap-1">
              <div className="w-2 h-2 rounded bg-green-500"></div>
              <span>Baixo (até 40%)</span>
            </div>
            <div className="flex items-center gap-1">
              <div className="w-2 h-2 rounded bg-yellow-400"></div>
              <span>Médio (40-70%)</span>
            </div>
            <div className="flex items-center gap-1">
              <div className="w-2 h-2 rounded bg-red-500"></div>
              <span>Alto (acima de 70%)</span>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  );
};