export const mockKPIData = {
  totalPatients: 1250000,
  totalHospitals: 4580,
  totalDoctors: 125000,
  averageOccupancy: 78,
};

export const mockDiseaseData = [
  { name: "Hipertensão", value: 25000, code: "I10" },
  { name: "Diabetes", value: 18500, code: "E11" },
  { name: "Pneumonia", value: 15200, code: "J18" },
  { name: "Infarto", value: 12800, code: "I21" },
  { name: "COVID-19", value: 8900, code: "U07" },
];

export const mockSpecialtyData = [
  { specialty: "Cardiologia", doctors: 1200, patients: 25000 },
  { specialty: "Pneumologia", doctors: 800, patients: 18000 },
  { specialty: "Endocrinologia", doctors: 650, patients: 15500 },
  { specialty: "Neurologia", doctors: 900, patients: 12000 },
  { specialty: "Ortopedia", doctors: 1100, patients: 22000 },
];

export const mockHospitalData = [
  { 
    id: 1, 
    name: "Hospital das Clínicas", 
    city: "São Paulo", 
    state: "SP", 
    doctors: 450, 
    capacity: 800, 
    occupancy: 85 
  },
  { 
    id: 2, 
    name: "Hospital Albert Einstein", 
    city: "São Paulo", 
    state: "SP", 
    doctors: 320, 
    capacity: 600, 
    occupancy: 72 
  },
  { 
    id: 3, 
    name: "Hospital Sírio-Libanês", 
    city: "São Paulo", 
    state: "SP", 
    doctors: 280, 
    capacity: 500, 
    occupancy: 68 
  },
  { 
    id: 4, 
    name: "Hospital Copa D'Or", 
    city: "Rio de Janeiro", 
    state: "RJ", 
    doctors: 200, 
    capacity: 400, 
    occupancy: 78 
  },
];

export const mockAgeGenderData = [
  { ageGroup: "0-18", male: 12000, female: 11500 },
  { ageGroup: "19-35", male: 18500, female: 19200 },
  { ageGroup: "36-50", male: 22000, female: 21800 },
  { ageGroup: "51-65", male: 28500, female: 26000 },
  { ageGroup: "65+", male: 35000, female: 38000 },
];

export const mockMonthlyData = [
  { month: "Jan", patients: 95000, revenue: 12500000 },
  { month: "Fev", patients: 98000, revenue: 13200000 },
  { month: "Mar", patients: 102000, revenue: 14100000 },
  { month: "Abr", patients: 106000, revenue: 14800000 },
  { month: "Mai", patients: 108000, revenue: 15200000 },
  { month: "Jun", patients: 112000, revenue: 15800000 },
];

export const mockImportHistory = [
  {
    id: "1",
    fileName: "hospitais_sp_2025.json",
    dataType: "hospitals" as const,
    fileFormat: "json" as const,
    uploadDate: new Date("2025-09-20T10:30:00"),
    status: "success" as const,
    summary: "1000 de 1000 registros importados com sucesso",
    totalRecords: 1000,
    successfulRecords: 1000,
    errorRecords: 0,
    description: "Dados dos hospitais do estado de São Paulo - Setembro/2025"
  },
  {
    id: "2",
    fileName: "medicos_rj_setembro.xml",
    dataType: "doctors" as const,
    fileFormat: "xml" as const,
    uploadDate: new Date("2025-09-20T09:15:00"),
    status: "warning" as const,
    summary: "980 de 1000 registros importados. 15 duplicados ignorados, 5 com erro",
    totalRecords: 1000,
    successfulRecords: 980,
    errorRecords: 5,
    duplicateRecords: 15,
    description: "Médicos do Rio de Janeiro - Setembro 2025",
    errors: [
      {
        line: 52,
        field: "data_nascimento",
        message: "O valor '30/02/2001' não é uma data válida",
        data: "30/02/2001"
      },
      {
        line: 115,
        field: "municipio_id",
        message: "O município com ID '9999' não foi encontrado na base de dados",
        data: "9999"
      },
      {
        line: 203,
        field: "crm_numero",
        message: "CRM já existe na base de dados",
        data: "CRM/RJ 123456"
      },
      {
        line: 456,
        field: "especialidade_id",
        message: "Especialidade com ID '999' não encontrada",
        data: "999"
      },
      {
        line: 789,
        field: "email",
        message: "Formato de email inválido",
        data: "medico@email@invalido.com"
      }
    ]
  },
  {
    id: "3",
    fileName: "pacientes_mg_agosto.csv",
    dataType: "patients" as const,
    fileFormat: "csv" as const,
    uploadDate: new Date("2025-09-19T16:45:00"),
    status: "error" as const,
    summary: "Arquivo com formato inválido ou corrompido",
    totalRecords: 0,
    successfulRecords: 0,
    errorRecords: 1,
    description: "Pacientes de Minas Gerais - Agosto 2025",
    errors: [
      {
        line: 1,
        field: "arquivo",
        message: "Arquivo CSV corrompido ou formato inválido. Verifique a codificação e separadores",
        data: "N/A"
      }
    ]
  },
  {
    id: "4",
    fileName: "cid10_atualizacao.json",
    dataType: "cid10" as const,
    fileFormat: "json" as const,
    uploadDate: new Date("2025-09-19T14:20:00"),
    status: "processing" as const,
    summary: "Processando arquivo...",
    description: "Atualização da tabela CID-10 - versão 2025"
  },
  {
    id: "5",
    fileName: "municipios_brasil.xml",
    dataType: "locations" as const,
    fileFormat: "xml" as const,
    uploadDate: new Date("2025-09-18T11:30:00"),
    status: "success" as const,
    summary: "5570 de 5570 municípios importados com sucesso",
    totalRecords: 5570,
    successfulRecords: 5570,
    errorRecords: 0,
    description: "Dados completos dos municípios brasileiros - IBGE 2025"
  }
];