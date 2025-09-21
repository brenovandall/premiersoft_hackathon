import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useState } from "react";

const statesData = [
  { id: "AC", name: "Acre", value: 1250, path: "M150,250 L180,250 L180,280 L150,280 Z" },
  { id: "AL", name: "Alagoas", value: 3500, path: "M450,200 L470,200 L470,220 L450,220 Z" },
  { id: "AP", name: "Amapá", value: 890, path: "M300,50 L320,50 L320,80 L300,80 Z" },
  { id: "AM", name: "Amazonas", value: 5200, path: "M150,120 L250,120 L250,200 L150,200 Z" },
  { id: "BA", name: "Bahia", value: 12400, path: "M380,160 L450,160 L450,220 L380,220 Z" },
  { id: "CE", name: "Ceará", value: 8900, path: "M420,100 L470,100 L470,140 L420,140 Z" },
  { id: "DF", name: "Distrito Federal", value: 3200, path: "M350,200 L360,200 L360,210 L350,210 Z" },
  { id: "ES", name: "Espírito Santo", value: 4100, path: "M430,230 L450,230 L450,250 L430,250 Z" },
  { id: "GO", name: "Goiás", value: 6800, path: "M320,180 L380,180 L380,240 L320,240 Z" },
  { id: "MA", name: "Maranhão", value: 6200, path: "M360,80 L420,80 L420,140 L360,140 Z" },
  { id: "MT", name: "Mato Grosso", value: 3400, path: "M280,160 L340,160 L340,220 L280,220 Z" },
  { id: "MS", name: "Mato Grosso do Sul", value: 2800, path: "M300,220 L360,220 L360,280 L300,280 Z" },
  { id: "MG", name: "Minas Gerais", value: 18500, path: "M360,200 L430,200 L430,260 L360,260 Z" },
  { id: "PA", name: "Pará", value: 7800, path: "M250,80 L360,80 L360,160 L250,160 Z" },
  { id: "PB", name: "Paraíba", value: 3900, path: "M460,120 L480,120 L480,140 L460,140 Z" },
  { id: "PR", name: "Paraná", value: 11200, path: "M360,260 L420,260 L420,300 L360,300 Z" },
  { id: "PE", name: "Pernambuco", value: 9600, path: "M430,140 L480,140 L480,180 L430,180 Z" },
  { id: "PI", name: "Piauí", value: 3100, path: "M380,120 L430,120 L430,160 L380,160 Z" },
  { id: "RJ", name: "Rio de Janeiro", value: 16800, path: "M420,240 L450,240 L450,270 L420,270 Z" },
  { id: "RN", name: "Rio Grande do Norte", value: 3600, path: "M450,100 L480,100 L480,130 L450,130 Z" },
  { id: "RS", name: "Rio Grande do Sul", value: 11400, path: "M340,300 L400,300 L400,350 L340,350 Z" },
  { id: "RO", name: "Rondônia", value: 1800, path: "M220,180 L280,180 L280,220 L220,220 Z" },
  { id: "RR", name: "Roraima", value: 620, path: "M200,30 L250,30 L250,80 L200,80 Z" },
  { id: "SC", name: "Santa Catarina", value: 7200, path: "M360,280 L420,280 L420,320 L360,320 Z" },
  { id: "SP", name: "São Paulo", value: 42000, path: "M380,240 L440,240 L440,290 L380,290 Z" },
  { id: "SE", name: "Sergipe", value: 2300, path: "M440,180 L460,180 L460,200 L440,200 Z" },
  { id: "TO", name: "Tocantins", value: 1600, path: "M330,120 L380,120 L380,180 L330,180 Z" },
];

export const InteractiveBrazilMap = () => {
  const [hoveredState, setHoveredState] = useState<string | null>(null);
  const [mousePosition, setMousePosition] = useState({ x: 0, y: 0 });

  const maxValue = Math.max(...statesData.map(s => s.value));

  const getColor = (value: number) => {
    const intensity = value / maxValue;
    if (intensity > 0.8) return "#dc2626";
    if (intensity > 0.6) return "#f87171";
    if (intensity > 0.4) return "#fbbf24";
    if (intensity > 0.2) return "#a3e635";
    return "#22c55e";
  };

  const getStrokeColor = (isHovered: boolean) => {
    return isHovered ? "#1f2937" : "#ffffff";
  };

  const handleMouseMove = (event: React.MouseEvent) => {
    setMousePosition({ x: event.clientX, y: event.clientY });
  };

  const hoveredStateData = hoveredState ? statesData.find(s => s.id === hoveredState) : null;

  return (
    <Card className="bg-gradient-card shadow-card border-0">
      <CardHeader>
        <CardTitle className="text-lg font-semibold text-foreground">
          Mapa Interativo do Brasil - Internações por Estado
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div className="relative" onMouseMove={handleMouseMove}>
          <svg
            viewBox="0 0 500 400"
            className="w-full h-96 border rounded-lg bg-slate-50"
          >
            {/* Estados do mapa */}
            {statesData.map((state) => (
              <g key={state.id}>
                <path
                  d={state.path}
                  fill={getColor(state.value)}
                  stroke={getStrokeColor(hoveredState === state.id)}
                  strokeWidth={hoveredState === state.id ? "3" : "1"}
                  className="cursor-pointer transition-all duration-200"
                  onMouseEnter={() => setHoveredState(state.id)}
                  onMouseLeave={() => setHoveredState(null)}
                />
                {/* Label do estado */}
                <text
                  x={state.path.split(' ')[0].split(',')[0].replace('M', '')}
                  y={state.path.split(' ')[0].split(',')[1]}
                  dx="15"
                  dy="15"
                  textAnchor="middle"
                  dominantBaseline="central"
                  className="text-xs font-bold fill-white pointer-events-none"
                  style={{ 
                    textShadow: '1px 1px 2px rgba(0,0,0,0.8)',
                    fontSize: '10px'
                  }}
                >
                  {state.id}
                </text>
              </g>
            ))}
          </svg>

          {/* Tooltip */}
          {hoveredState && hoveredStateData && (
            <div
              className="fixed bg-gray-900 text-white p-3 rounded-lg shadow-lg z-50 pointer-events-none border border-gray-600"
              style={{
                left: mousePosition.x + 10,
                top: mousePosition.y - 70,
                transform: 'translateX(-50%)'
              }}
            >
              <div className="text-sm font-semibold text-yellow-300">{hoveredStateData.name}</div>
              <div className="text-xs text-gray-300">Estado: {hoveredStateData.id}</div>
              <div className="text-xs text-white">
                Internações: <span className="font-semibold">{hoveredStateData.value.toLocaleString()}</span>
              </div>
              <div 
                className="text-xs px-2 py-1 rounded mt-2 text-center font-medium"
                style={{ backgroundColor: getColor(hoveredStateData.value) }}
              >
                {hoveredStateData.value > maxValue * 0.8 ? 'Nível Alto' :
                 hoveredStateData.value > maxValue * 0.6 ? 'Nível Médio-Alto' :
                 hoveredStateData.value > maxValue * 0.4 ? 'Nível Médio' :
                 hoveredStateData.value > maxValue * 0.2 ? 'Nível Baixo-Médio' : 'Nível Baixo'}
              </div>
            </div>
          )}
        </div>

        {/* Legenda detalhada */}
        <div className="mt-4 space-y-3">
          <div className="flex items-center justify-between text-xs text-muted-foreground">
            <div className="flex items-center gap-4">
              <div className="flex items-center gap-2">
                <div className="w-4 h-3 rounded" style={{ backgroundColor: "#22c55e" }}></div>
                <span>Baixo (≤{Math.round(maxValue * 0.2).toLocaleString()})</span>
              </div>
              <div className="flex items-center gap-2">
                <div className="w-4 h-3 rounded" style={{ backgroundColor: "#a3e635" }}></div>
                <span>Baixo-Médio</span>
              </div>
              <div className="flex items-center gap-2">
                <div className="w-4 h-3 rounded" style={{ backgroundColor: "#fbbf24" }}></div>
                <span>Médio</span>
              </div>
              <div className="flex items-center gap-2">
                <div className="w-4 h-3 rounded" style={{ backgroundColor: "#f87171" }}></div>
                <span>Médio-Alto</span>
              </div>
              <div className="flex items-center gap-2">
                <div className="w-4 h-3 rounded" style={{ backgroundColor: "#dc2626" }}></div>
                <span>Alto (≥{Math.round(maxValue * 0.8).toLocaleString()})</span>
              </div>
            </div>
          </div>
          
          {/* Estatísticas do mapa */}
          <div className="flex items-center justify-between text-xs text-muted-foreground bg-accent/20 p-3 rounded-lg">
            <div>
              <span className="font-medium">Total Nacional: </span>
              <span className="font-semibold text-foreground">
                {statesData.reduce((sum, state) => sum + state.value, 0).toLocaleString()} internações
              </span>
            </div>
            <div className="text-right">
              <div>💡 <span className="font-medium">Passe o mouse sobre os estados</span></div>
              <div>📊 <span>Clique para mais detalhes</span></div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  );
};