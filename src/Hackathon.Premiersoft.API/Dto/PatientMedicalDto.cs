namespace Hackathon.Premiersoft.API.Dto
{
    public class PatientDemographicDto
    {
        public string AgeGroup { get; set; } = string.Empty;
        public int Male { get; set; }
        public int Female { get; set; }
        public int Total => Male + Female;
    }

    public class DoctorSpecialtyStatsDto
    {
        public string Specialty { get; set; } = string.Empty;
        public int Doctors { get; set; }
        public int Patients { get; set; }
        public decimal DoctorPatientRatio => Doctors > 0 ? (decimal)Patients / Doctors : 0;
    }

    public class DoctorSearchDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public List<string> Hospitals { get; set; } = new();
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }

    public class PatientStatsDto
    {
        public int TotalPatients { get; set; }
        public int MalePatients { get; set; }
        public int FemalePatients { get; set; }
        public decimal MalePercentage => TotalPatients > 0 ? (decimal)MalePatients / TotalPatients * 100 : 0;
        public decimal FemalePercentage => TotalPatients > 0 ? (decimal)FemalePatients / TotalPatients * 100 : 0;
    }
}