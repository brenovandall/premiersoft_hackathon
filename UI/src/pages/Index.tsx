import { useState } from "react";
import { DashboardLayout } from "@/components/dashboard/DashboardLayout";
import { OverviewView } from "@/components/dashboard/OverviewView";
import { HospitalView } from "@/components/dashboard/HospitalView";
import { PatientsView } from "@/components/dashboard/PatientsView";

const Index = () => {
  const [activeView, setActiveView] = useState<"overview" | "hospital" | "patients">("overview");

  const renderView = () => {
    switch (activeView) {
      case "overview":
        return <OverviewView />;
      case "hospital":
        return <HospitalView />;
      case "patients":
        return <PatientsView />;
      default:
        return <OverviewView />;
    }
  };

  return (
    <DashboardLayout activeView={activeView} onViewChange={setActiveView}>
      {renderView()}
    </DashboardLayout>
  );
};

export default Index;
