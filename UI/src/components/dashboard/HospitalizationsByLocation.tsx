import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useState } from "react";
import { ChevronDown, ChevronUp } from "lucide-react";

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
  const [expandedStates, setExpandedStates] = useState<Set<string>>(new Set());

  const toggleState = (stateCode: string) => {
    const newExpanded = new Set(expandedStates);
    if (newExpanded.has(stateCode)) {
      newExpanded.delete(stateCode);
    } else {
      newExpanded.add(stateCode);
    }
    setExpandedStates(newExpanded);
  };

  const getIntensityColor = (cases: number, maxCases: number) => {
    const intensity = cases / maxCases;
    if (intensity > 0.8) return "bg-red-500";
    if (intensity > 0.6) return "bg-red-400";
    if (intensity > 0.4) return "bg-yellow-400";
    if (intensity > 0.2) return "bg-green-400";
    return "bg-green-500";
  };

  const maxStateCases = Math.max(...hospitalizationData.map(s => s.total));

  return (
    <Card className="bg-gradient-card shadow-card border-0">
      <CardHeader>
        <CardTitle className="text-lg font-semibold text-foreground">
          Total de Internações por Estado/Município
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div className="space-y-2">
          {hospitalizationData.map((stateData) => {
            const isExpanded = expandedStates.has(stateData.stateCode);
            const maxMunicipalityCases = Math.max(...stateData.municipalities.map(m => m.cases));
            
            return (
              <div key={stateData.stateCode} className="border rounded-lg overflow-hidden">
                <div
                  className="p-4 cursor-pointer hover:bg-accent/50 transition-colors flex items-center justify-between"
                  onClick={() => toggleState(stateData.stateCode)}
                >
                  <div className="flex items-center gap-3">
                    <div className={`w-3 h-3 rounded ${getIntensityColor(stateData.total, maxStateCases)}`}></div>
                    <div>
                      <span className="font-medium text-foreground">{stateData.state}</span>
                      <span className="text-sm text-muted-foreground ml-2">({stateData.stateCode})</span>
                    </div>
                  </div>
                  <div className="flex items-center gap-2">
                    <span className="font-semibold text-foreground">
                      {stateData.total.toLocaleString()} casos
                    </span>
                    {isExpanded ? (
                      <ChevronUp className="w-4 h-4 text-muted-foreground" />
                    ) : (
                      <ChevronDown className="w-4 h-4 text-muted-foreground" />
                    )}
                  </div>
                </div>
                
                {isExpanded && (
                  <div className="px-4 pb-4 bg-accent/20">
                    <div className="space-y-2">
                      <h4 className="text-sm font-medium text-muted-foreground mb-3">
                        Principais Municípios:
                      </h4>
                      {stateData.municipalities.map((municipality, index) => (
                        <div key={index} className="flex items-center justify-between py-2 px-3 bg-background rounded">
                          <div className="flex items-center gap-2">
                            <div className={`w-2 h-2 rounded ${getIntensityColor(municipality.cases, maxMunicipalityCases)}`}></div>
                            <span className="text-sm text-foreground">{municipality.name}</span>
                          </div>
                          <span className="text-sm font-medium text-foreground">
                            {municipality.cases.toLocaleString()}
                          </span>
                        </div>
                      ))}
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
              <span>Baixo</span>
            </div>
            <div className="flex items-center gap-1">
              <div className="w-2 h-2 rounded bg-yellow-400"></div>
              <span>Médio</span>
            </div>
            <div className="flex items-center gap-1">
              <div className="w-2 h-2 rounded bg-red-500"></div>
              <span>Alto</span>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  );
};