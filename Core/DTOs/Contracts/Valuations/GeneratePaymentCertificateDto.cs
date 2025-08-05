namespace Core.DTOs.Contracts.Valuations;

public class GeneratePaymentCertificateDto
{
    public string CertificateNumber { get; set; } = string.Empty;
    public string GeneratedBy { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
}