namespace EMS.Models
{
    public class AzureBlobSettings
    {
        public const string SectionName = "AzureBlobStorage";
        public string ConnectionString { get; set; } = string.Empty;
        public string ResumesContainer { get; set; } = string.Empty;
        public string PhotoContainer { get; set; } = string.Empty;
        public string AadharContainer { get; set; } = string.Empty;
        public string PanContainer { get; set; } = string.Empty;
        public string PayslipContainer   { get; set; } = string.Empty;
        public string CertificateContainer { get; set; } = string.Empty;
        public string LogContainer   { get; set; } = string.Empty;
    }
}
