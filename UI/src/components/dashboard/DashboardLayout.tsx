import { ReactNode } from "react";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { BarChart3, Hospital, Users, MapPin, Upload } from "lucide-react";
import { Link } from "react-router-dom";

interface DashboardLayoutProps {
  children: ReactNode;
  activeView: "overview" | "hospital" | "patients";
  onViewChange: (view: "overview" | "hospital" | "patients") => void;
}

export const DashboardLayout = ({ children, activeView, onViewChange }: DashboardLayoutProps) => {
  const navItems = [
    { id: "overview", label: "Visão Geral", icon: BarChart3 },
    { id: "hospital", label: "Hospitais", icon: Hospital },
    { id: "patients", label: "Pacientes & Médicos", icon: Users },
  ] as const;

  return (
    <div className="min-h-screen bg-gradient-subtle">
      <header className="bg-card shadow-card border-b">
        <div className="container mx-auto px-6 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center space-x-3">
              <div className="p-2 bg-gradient-primary rounded-lg">
                <MapPin className="h-6 w-6 text-white" />
              </div>
              <div>
                <h1 className="text-2xl font-bold text-foreground">Dashboard de Saúde</h1>
                <p className="text-sm text-muted-foreground">Insights para Gestores de Saúde</p>
              </div>
            </div>
            <Link to="/import">
              <Button variant="outline" className="flex items-center gap-2">
                <Upload className="h-4 w-4" />
                Importar Dados
              </Button>
            </Link>
          </div>
        </div>
      </header>

      <nav className="bg-card shadow-card border-b">
        <div className="container mx-auto px-6">
          <div className="flex space-x-1">
            {navItems.map(({ id, label, icon: Icon }) => (
              <Button
                key={id}
                variant={activeView === id ? "default" : "ghost"}
                onClick={() => onViewChange(id)}
                className={cn(
                  "px-6 py-3 rounded-none border-b-2 border-transparent transition-smooth",
                  activeView === id && "border-primary bg-primary text-primary-foreground"
                )}
              >
                <Icon className="h-4 w-4 mr-2" />
                {label}
              </Button>
            ))}
          </div>
        </div>
      </nav>

      <main className="container mx-auto px-6 py-8">
        {children}
      </main>
    </div>
  );
};